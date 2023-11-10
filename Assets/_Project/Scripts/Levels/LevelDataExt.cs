using System.Collections.Generic;
using DaftAppleGames.RetroRacketRevolution.Bonuses;
using DaftAppleGames.RetroRacketRevolution.Bricks;
using Sirenix.OdinInspector;
using System.IO;
using DaftAppleGames.RetroRacketRevolution.Utils;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.Levels
{
    public class LevelDataExt
    {
        [BoxGroup("Level Details")] public string levelName;
        [BoxGroup("Level Details")] public string levelFileName;
        [BoxGroup("Level Details")] public string levelAuthor;
        [BoxGroup("Level Details")] public string levelBackgroundName;
        [BoxGroup("Level Details")] public int levelBackgroundIndex;
        [BoxGroup("Enemies")] public int maxEnemies = 0;
        [BoxGroup("Enemies")] public float minTimeBetweenEnemies = 5.0f;
        [BoxGroup("Enemies")] public float maxTimeBetweenEnemies = 100.0f;
        [BoxGroup("Level Data")]
        [SerializeField] public Rows BrickDataArray = new Rows();

        [BoxGroup("Level Layout")] private const int numberOfRows = 12;
        [BoxGroup("Level Layout")] private const int numberOfBricksPerRow = 15;

#if UNITY_EDITOR
        public static string OgLevelDataPath = "E:\\Dev\\DAG\\Retro Racket Revolution\\Assets\\_Project\\Resources\\LevelData";
        public static string CustomLevelDataPath = "E:\\Dev\\DAG\\Retro Racket Revolution\\Assets\\_Project\\Resources\\CustomLevelData";
#else
        public static string OgLevelDataPath = Path.Combine(Path.GetFullPath("./"), "LevelData");
        public static string CustomLevelDataPath = Path.Combine(Path.GetFullPath("./"), "CustomLevelData");
#endif
        /// <summary>
        /// Add / update a brick
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// Where (x) is the row, (y) is the number of bricks from the left of the row
        /// <param name="brickData"></param>
        public void SetBrick(int row, int column, BrickData brickData)
        {
            if (row < 0 || column < 0 || row > numberOfRows || column > numberOfBricksPerRow)
            {
                Debug.Log("Requested brick array index is out of range");
                return;
            }
            BrickDataArray.RowArray[row].RowBricks[column] = brickData;
        }

        /// <summary>
        /// Sets a slot IsEmpty state to given boolean
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="isEmpty"></param>
        public void SetSlotState(int row, int column, bool isEmpty)
        {
            if (row < 0 || column < 0 || row > numberOfRows || column > numberOfBricksPerRow)
            {
                Debug.Log("Requested brick array index is out of range");
                return;
            }
            BrickDataArray.RowArray[row].RowBricks[column].IsEmptySlot = isEmpty;
        }

        /// <summary>
        /// Rows consist of a number of rows
        /// </summary>
        [System.Serializable]
        public class Rows
        {
            public Row[] RowArray = new Row[numberOfRows];
        }

        /// <summary>
        /// A row consists of n "columns" of bricks
        /// </summary>
        [System.Serializable]
        public class Row
        {
            public BrickData[] RowBricks = new BrickData[numberOfBricksPerRow];
        }

        /// <summary>
        /// Initialise the scriptable object instance with empty bricks
        /// </summary>
        public void Init()
        {
            for (int currRow = 0; currRow < numberOfRows; currRow++)
            {
                BrickDataArray.RowArray[currRow] = new Row();
                for (int currCol = 0; currCol < numberOfBricksPerRow; currCol++)
                {
                    BrickDataArray.RowArray[currRow].RowBricks[currCol] =
                        new BrickData(BrickType.Normal, Color.white, BonusType.None, true, currRow, currCol);
                }
            }
        }

        /// <summary>
        /// Prints the Brick Array to the Debug log
        /// </summary>
        public static void Print(LevelDataExt levelData)
        {
            for (int currRow = 0; currRow < numberOfRows; currRow++)
            {
                for (int currCol = 0; currCol < numberOfBricksPerRow; currCol++)
                {
                    BrickData brickData = levelData.BrickDataArray.RowArray[currRow].RowBricks[currCol];
                    Debug.Log($"Brick: ({brickData.ColumnNumber}, {brickData.RowNumber}), Color: {brickData.BrickColor.ToString()}. Type: {brickData.BrickType}, Bonus: {brickData.BrickBonus}");
                }
            }
        }

        /// <summary>
        /// Deletes the given instance file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="isCustomLevels"></param>
        public static void DeleteLevelInstanceFile(string fileName, bool isCustomLevels)
        {
            string levelPath = isCustomLevels ? CustomLevelDataPath : OgLevelDataPath;
            string levelFilePath = Path.Combine(levelPath, Path.ChangeExtension(fileName, ".json"));

            File.Delete(levelFilePath);
            Debug.Log($"File deleted: {levelFilePath}");
        }

        /// <summary>
        /// Return a compressed Base64 encoded version of the level
        /// </summary>
        /// <returns></returns>
        public string BaseEncodeLevel()
        {
            // Serialize the data
            string json = JsonUtility.ToJson(this, false);

            // Base64 encode
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(json);
            return System.Convert.ToBase64String(plainTextBytes).Compress();
        }

        /// <summary>
        /// Decodes a compressed Base64 encoded level, returns a LevelDataExt instance
        /// </summary>
        /// <param name="encodedCompressedLevel"></param>
        /// <returns></returns>
        public static LevelDataExt BaseDecodeLevel(string encodedCompressedLevel)
        {
            byte[] base64EncodedBytes = System.Convert.FromBase64String(encodedCompressedLevel.Decompress());
            LevelDataExt newLevelData = JsonUtility.FromJson<LevelDataExt>(System.Text.Encoding.UTF8.GetString(base64EncodedBytes));
            return newLevelData;
        }

        /// <summary>
        /// Decodes the level and saves to disk as JSON
        /// </summary>
        /// <param name="encodedLevel"></param>
        public void BaseDecodeLevelAndSave(string encodedLevel)
        {
            LevelDataExt newLevelDataExt = BaseDecodeLevel(encodedLevel);
            newLevelDataExt.SaveInstanceToFile(newLevelDataExt.levelFileName, true);
        }

        /// <summary>
        /// Save the instance to a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="isCustomLevels"></param>
        public void SaveInstanceToFile(string fileName, bool isCustomLevels)
        {
            string levelPath = isCustomLevels ? CustomLevelDataPath : OgLevelDataPath;

            if (!File.Exists(levelPath))
            {
                Debug.Log($"Save: Creating folder: {OgLevelDataPath}");
                Directory.CreateDirectory(OgLevelDataPath);
            }

            // Derive the full save file path
            string levelFilePath = Path.Combine(levelPath, Path.ChangeExtension(fileName, ".json"));

            // Add / update the filename
            this.levelFileName = fileName;

           // Serialise the data
            string json = JsonUtility.ToJson(this, true);

            // Write the JSON to file
            Debug.Log($"Save: Saving to: {levelFilePath}");
            using (StreamWriter outputFile = new StreamWriter(levelFilePath, false))
            {
                outputFile.WriteLine(json);
            }
        }

        /// <summary>
        /// Loads an instance from a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="isCustomLevels"></param>
        /// <returns></returns>
        public static LevelDataExt LoadInstanceFromFile(string fileName, bool isCustomLevels)
        {
            string json;
#if PLATFORM_ANDROID
            TextAsset level = Resources.Load<TextAsset>($"LevelData\\{fileName}");
            json = level.text;
#else
            string levelPath = isCustomLevels ? CustomLevelDataPath : OgLevelDataPath;

            string levelFilePath = Path.Combine(levelPath, Path.ChangeExtension(fileName, ".json"));
            Debug.Log($"Loaded data from file: {levelFilePath}");

            if (!File.Exists(levelFilePath))
            {
                return null;
            }

            // Write the JSON to file
            using (StreamReader inputFile = new StreamReader(levelFilePath))
            {
                json = inputFile.ReadToEnd();
            }
#endif
            LevelDataExt levelDataExt = JsonUtility.FromJson<LevelDataExt>(json);
            
            // Print(levelDataExt);
            return levelDataExt;
        }

        /// <summary>
        /// Gets a list of all files in the saved level folder
        /// </summary>
        /// <returns></returns>
        public static List<string> GetLevelsNames(bool isCustomLevels)
        {
            List<string> levelPathsList = new List<string>();

            string levelPath = isCustomLevels ? CustomLevelDataPath : OgLevelDataPath;
#if PLATFORM_ANDROID
            TextAsset[] allLevels = Resources.LoadAll<TextAsset>("LevelData");

            // Load levels from Resource folder
            foreach (TextAsset level in allLevels)
            {
                levelPathsList.Add(level.name);
            }
#else
            foreach (string currFile in Directory.GetFiles(levelPath, "*.json"))
            {
                levelPathsList.Add(Path.GetFileName(currFile));
            }
#endif
                levelPathsList.Sort();
            return levelPathsList;
        }
    }
}

using System;
using System.Collections.Generic;
using DaftAppleGames.RetroRacketRevolution.Bonuses;
using DaftAppleGames.RetroRacketRevolution.Bricks;
using Sirenix.OdinInspector;
using System.IO;
using DaftAppleGames.RetroRacketRevolution.LevelEditor;
using DaftAppleGames.RetroRacketRevolution.Utils;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.Levels
{
    [Serializable] public class LevelDataExt
    {
        [BoxGroup("Level Details")] public string levelName;
        [BoxGroup("Level Details")] public string levelFileName;
        [BoxGroup("Level Details")] public string levelAuthor;
        [BoxGroup("Level Details")] public int levelBackdropSceneIndex;
        [BoxGroup("Level Details")] public int levelBackgroundMusicIndex;
        [BoxGroup("Level Details")] public bool isBossLevel;
        [BoxGroup("Level Details")] public int levelBossIndex;
        [BoxGroup("Enemies")] public int maxEnemies = 0;
        [BoxGroup("Enemies")] public float minTimeBetweenEnemies = 5.0f;
        [BoxGroup("Enemies")] public float maxTimeBetweenEnemies = 100.0f;
        [BoxGroup("Level Data")] public Rows brickDataArray = new Rows();

        [BoxGroup("Level Layout")] private const int numberOfRows = 12;
        [BoxGroup("Level Layout")] private const int numberOfBricksPerRow = 15;

#if UNITY_EDITOR
        public static string OgLevelDataPath =
            "E:\\Dev\\DAG\\Retro Racket Revolution\\Assets\\_Project\\Resources\\LevelData";
        public static string CustomLevelDataPath =
            "E:\\Dev\\DAG\\Retro Racket Revolution\\Assets\\_Project\\Resources\\CustomLevelData";
#else
        public static string OgLevelDataPath = Path.Combine(Application.persistentDataPath, "LevelData");
        public static string CustomLevelDataPath = Path.Combine(Application.persistentDataPath, "CustomLevelData");
#endif
        /// <summary>
        /// Add / update a brick
        /// </summary>
        public void SetBrick(int row, int column, BrickData brickData)
        {
            if (row < 0 || column < 0 || row > numberOfRows || column > numberOfBricksPerRow)
            {
                Debug.Log("Requested brick array index is out of range");
                return;
            }

            brickDataArray.rowArray[row].rowBricks[column] = brickData;
        }

        /// <summary>
        /// Sets a slot IsEmpty state to given boolean
        /// </summary>
        public void SetSlotState(int row, int column, bool isEmpty)
        {
            if (row < 0 || column < 0 || row > numberOfRows || column > numberOfBricksPerRow)
            {
                Debug.Log("Requested brick array index is out of range");
                return;
            }

            brickDataArray.rowArray[row].rowBricks[column].isEmptySlot = isEmpty;
        }

        /// <summary>
        /// Rows consist of a number of rows
        /// </summary>
        [Serializable] public class Rows
        {
            public Row[] rowArray = new Row[numberOfRows];
        }

        /// <summary>
        /// A row consists of n "columns" of bricks
        /// </summary>
        [Serializable] public class Row
        {
            public BrickData[] rowBricks = new BrickData[numberOfBricksPerRow];
        }

        /// <summary>
        /// Initialise the scriptable object instance with empty bricks
        /// </summary>
        public void Init()
        {
            for (int currRow = 0; currRow < numberOfRows; currRow++)
            {
                brickDataArray.rowArray[currRow] = new Row();
                for (int currCol = 0; currCol < numberOfBricksPerRow; currCol++)
                {
                    brickDataArray.rowArray[currRow].rowBricks[currCol] =
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
                    BrickData brickData = levelData.brickDataArray.rowArray[currRow].rowBricks[currCol];
                    Debug.Log(
                        $"Brick: ({brickData.columnNumber}, {brickData.rowNumber}), Color: {brickData.brickColor.ToString()}. Type: {brickData.brickType}, Bonus: {brickData.brickBonus}");
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
        public string BaseEncodeLevel()
        {
            // Serialize the data
            string json = JsonUtility.ToJson(this, false);

            // Base64 encode
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(json);
            return Convert.ToBase64String(plainTextBytes).Compress();
        }

        /// <summary>
        /// Decodes a compressed Base64 encoded level, returns a LevelDataExt instance
        /// </summary>
        public static LevelDataExt BaseDecodeLevel(string encodedCompressedLevel)
        {
            byte[] base64EncodedBytes = Convert.FromBase64String(encodedCompressedLevel.Decompress());
            LevelDataExt newLevelData =
                JsonUtility.FromJson<LevelDataExt>(System.Text.Encoding.UTF8.GetString(base64EncodedBytes));
            return newLevelData;
        }

        /// <summary>
        /// Decodes the level and saves to disk as JSON
        /// </summary>
        public void BaseDecodeLevelAndSave(string encodedLevel)
        {
            LevelDataExt newLevelDataExt = BaseDecodeLevel(encodedLevel);
            newLevelDataExt.SaveInstanceToFile(newLevelDataExt.levelFileName, true);
        }

        /// <summary>
        /// Save the instance to a file
        /// </summary>
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
            levelFileName = fileName;

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
        public static LevelDataExt LoadInstanceFromFile(string fileName, bool isCustomLevels)
        {
            string json;
            string levelPath = isCustomLevels ? CustomLevelDataPath : OgLevelDataPath;
            string levelFilePath = Path.Combine(levelPath, Path.ChangeExtension(fileName, ".json"));

            Debug.Log($"Loading data from file: {levelFilePath}");

            if (!File.Exists(levelFilePath))
            {
                return null;
            }

            // Write the JSON to file
            using (StreamReader inputFile = new StreamReader(levelFilePath))
            {
                json = inputFile.ReadToEnd();
            }

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
#if PLATFORM_ANDROID && !UNITY_EDITOR
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
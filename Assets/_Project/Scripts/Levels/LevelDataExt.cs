using System.Collections.Generic;
using DaftApplesGames.RetroRacketRevolution.Bonuses;
using DaftApplesGames.RetroRacketRevolution.Bricks;
using Sirenix.OdinInspector;
using System.IO;
using UnityEngine;

namespace DaftApplesGames.RetroRacketRevolution.Levels
{
    public class LevelDataExt
    {
        [BoxGroup("Level Details")] public string levelName;
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
        public static string LevelDataPath = "C:\\Games\\Retro Racket Revolution\\LevelData";
#else
         public static string LevelDataPath = Path.Combine(Path.GetFullPath("./"), "LevelData");
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
        /// Save the instance to a file
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveInstanceToFile(string fileName)
        {
            string path = Path.GetFullPath("./");
            string levelDataPath = Path.Combine(@path, "LevelData");
            if (!File.Exists(levelDataPath))
            {
                Debug.Log($"Save: Creating folder: {LevelDataPath}");
                Directory.CreateDirectory(LevelDataPath);
            }

            // Derive the full save file path
            string levelFilePath = Path.Combine(LevelDataPath, Path.ChangeExtension(fileName, ".json"));

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
        /// <returns></returns>
        public static LevelDataExt LoadInstanceFromFile(string fileName)
        {
            string levelFilePath = Path.Combine(LevelDataPath, Path.ChangeExtension(fileName, ".json"));
            if (!File.Exists(levelFilePath))
            {
                return null;
            }

            string json;
            // Write the JSON to file
            using (StreamReader inputFile = new StreamReader(levelFilePath))
            {
                json = inputFile.ReadToEnd();
            }

            LevelDataExt levelDataExt = JsonUtility.FromJson<LevelDataExt>(json);

            return levelDataExt;
        }

        /// <summary>
        /// Gets a list of all files in the saved level folder
        /// </summary>
        /// <returns></returns>
        public static string[] GetLevelsNames()
        {
            List<string> levelPathsList = new List<string>();
            foreach (string currFile in Directory.GetFiles(LevelDataPath))
            {
                levelPathsList.Add(Path.GetFileName(currFile));
            }
            levelPathsList.Sort();
            return levelPathsList.ToArray();
        }
    }
}

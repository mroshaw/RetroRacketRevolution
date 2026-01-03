using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.IO;
using DaftAppleGames.RetroRacketRevolution.Bonuses;
using DaftAppleGames.RetroRacketRevolution.Bricks;
using UnityEngine;
using DaftAppleGames.RetroRacketRevolution.Levels;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution.LevelEditor
{
    public class LevelEditorManager : MonoBehaviour
    {
        [BoxGroup("Background Sprites")] public LevelBackgroundSprites backgroundSprites;
        [BoxGroup("Bonuses")] public BonusData bonusData;
        [BoxGroup("Level Layout")] public int numberOfRows = 12;
        [BoxGroup("Level Layout")] public int numberOfBricksPerRow = 15;

        [BoxGroup("Events")] public UnityEvent<List<string>> levelsChangedEvent;

        // Level Public properties
        public int MaxEnemies { get; set; }
        public float MinEnemyTime { get; set; }
        public float MaxEnemyTime { get; set; }
        public string BackdropSceneName { get; set; } = "";
        public int BackdropSceneIndex { get; set; } = 0;
        public int BackgroundMusicIndex { get; set; }
        public string FileName { get; set; }
        public string LevelName { get; set; }
        public bool IsBossLevel { get; set; }
        public int LevelBossIndex { get; set; }
        public bool IsCustomLevel { get; set; }

        // Internal 2 dimension array of brick data
        public BrickData[,] BrickDataArray { get; set; }

        /// <summary>
        /// Set up the Level Editor Manager
        /// </summary>
        private void Awake()
        {
            // Initialise internal array
            BrickDataArray = new BrickData[numberOfRows, numberOfBricksPerRow];

            ClearData();
#if UNITY_EDITOR
            GetCurrentLevels(false);
#else
            GetCurrentLevels(true);
#endif
        }

        /// <summary>
        /// Refreshes the data set with empty bricks
        /// </summary>
        public void ClearData()
        {
            for (int currRow = 0; currRow < numberOfRows; currRow++)
            {
                for (int currCol = 0; currCol < numberOfBricksPerRow; currCol++)
                {
                    BrickDataArray[currRow, currCol] =
                        new BrickData(BrickType.Normal, Color.white, BonusType.None, true, currRow, currCol);
                }
            }

            LevelName = "";
            MaxEnemies = 0;
            MinEnemyTime = 0.0f;
            MaxEnemyTime = 0.0f;
            BackdropSceneName = "";
            BackgroundMusicIndex = 0;
            IsBossLevel = false;
            LevelBossIndex = 0;
            IsBossLevel = false;
        }

        /// <summary>
        /// Get the list of levels
        /// </summary>
        public List<string> GetCurrentLevels(bool isCustomLevels)
        {
            List<string> levels = new List<string>();

            foreach (string fileName in LevelDataExt.GetLevelsNames(isCustomLevels))
            {
                levels.Add(Path.GetFileNameWithoutExtension(fileName));
            }

            return levels;
        }

        /// <summary>
        /// Update a brick with the current selection, returns a brick
        /// specification string
        /// </summary>
        public void UpdateBrick(BrickData brickData)
        {
            int row = brickData.rowNumber;
            int column = brickData.columnNumber;

            if (brickData.HasBrickBonusChanged)
            {
                BrickDataArray[row, column].brickBonus = brickData.brickBonus;
            }

            if (brickData.HasBrickTypeChanged)
            {
                BrickDataArray[row, column].brickType = brickData.brickType;
            }

            if (brickData.HasIsEmptySlotChanged)
            {
                BrickDataArray[row, column].isEmptySlot = brickData.isEmptySlot;
            }

            if (brickData.HasBrickColorChanged)
            {
                BrickDataArray[row, column].brickColor = brickData.brickColor;
            }
        }

        /// <summary>
        /// Delete the actual selected level and refresh
        /// </summary>
        public void DeleteLevelFile(string fileName, bool isCustomLevels)
        {
            LevelDataExt.DeleteLevelInstanceFile(fileName, isCustomLevels);
            GetCurrentLevels(isCustomLevels);
        }

        /// <summary>
        /// Submits the level for review
        /// </summary>
        public void SubmitLevelFile(string levelName, string playerName, bool isCustomLevels)
        {
            LevelDataExt levelData = GetLevelAsLevelData();
            levelData.levelAuthor = playerName;
            string encodedLevel = levelData.BaseEncodeLevel();
            Debug.Log($"Encoded level: {encodedLevel}");
        }

        /// <summary>
        /// Loads a level from the given named asset file
        /// </summary>
        public void LoadLevelByName(string levelFileName, bool isCustomLevels)
        {
            Debug.Log($"Loading level: {levelFileName}...");
            LevelDataExt levelData = LevelDataExt.LoadInstanceFromFile(levelFileName, isCustomLevels);
            if (levelData == null)
            {
                Debug.Log($"Error: Level data file not found at path {levelFileName}");
            }

            Debug.Log($"Processing...");
            ProcessLevelData(levelData);
            Debug.Log($"Load complete!");
        }

        /// <summary>
        /// Generates an encoded view of current data
        /// </summary>
        public string GetLevelAsEncodedData(string playerName)
        {
            LevelDataExt levelData = GetLevelAsLevelData();
            levelData.levelAuthor = playerName;
            string encodedLevel = levelData.BaseEncodeLevel();
            return encodedLevel;
        }

        /// <summary>
        /// Loads a given encoded level into internal structures
        /// </summary>
        public void LoadLevelByEncodedData(string encodedLevelData)
        {
            LevelDataExt levelData = LevelDataExt.BaseDecodeLevel(encodedLevelData);
        }

        /// <summary>
        /// Save the current level
        /// </summary>
        public void SaveLevel()
        {
            LevelDataExt newLevelData = GetLevelAsLevelData();
            newLevelData.SaveInstanceToFile(FileName, IsCustomLevel);
            levelsChangedEvent?.Invoke(GetCurrentLevels(IsCustomLevel));
        }

        /// <summary>
        /// Creates an instance of LevelDataExt from current level design
        /// </summary>
        private LevelDataExt GetLevelAsLevelData()
        {
            // Init new instance
            LevelDataExt newLevelData = new LevelDataExt();
            newLevelData.Init();

            // Set level properties
            newLevelData.levelName = LevelName;
            newLevelData.levelBackdropSceneIndex = BackdropSceneIndex;
            newLevelData.levelBackgroundMusicIndex = BackgroundMusicIndex;
            newLevelData.maxEnemies = MaxEnemies;
            newLevelData.minTimeBetweenEnemies = MinEnemyTime;
            newLevelData.maxTimeBetweenEnemies = MaxEnemyTime;
            newLevelData.isBossLevel = IsBossLevel;
            newLevelData.levelBossIndex = LevelBossIndex;

            // Parse current level grid
            for (int currRow = 0; currRow < numberOfRows; currRow++)
            {
                for (int currCol = 0; currCol < numberOfBricksPerRow; currCol++)
                {
                    newLevelData.brickDataArray.rowArray[currRow].rowBricks[currCol].isEmptySlot =
                        BrickDataArray[currRow, currCol].isEmptySlot;
                    newLevelData.brickDataArray.rowArray[currRow].rowBricks[currCol].brickType =
                        BrickDataArray[currRow, currCol].brickType;
                    newLevelData.brickDataArray.rowArray[currRow].rowBricks[currCol].brickColor =
                        BrickDataArray[currRow, currCol].brickColor;
                    newLevelData.brickDataArray.rowArray[currRow].rowBricks[currCol].brickBonus =
                        BrickDataArray[currRow, currCol].brickBonus;
                    newLevelData.brickDataArray.rowArray[currRow].rowBricks[currCol].rowNumber =
                        BrickDataArray[currRow, currCol].rowNumber;
                    newLevelData.brickDataArray.rowArray[currRow].rowBricks[currCol].columnNumber =
                        BrickDataArray[currRow, currCol].columnNumber;
                }
            }

            return newLevelData;
        }

        /// <summary>
        /// Process level data into internal data structures
        /// </summary>
        private void ProcessLevelData(LevelDataExt levelData)
        {
            for (int currRow = 0; currRow < numberOfRows; currRow++)
            {
                for (int currCol = 0; currCol < numberOfBricksPerRow; currCol++)
                {
                    BrickDataArray[currRow, currCol].isEmptySlot =
                        levelData.brickDataArray.rowArray[currRow].rowBricks[currCol].isEmptySlot;
                    BrickDataArray[currRow, currCol].brickType =
                        levelData.brickDataArray.rowArray[currRow].rowBricks[currCol].brickType;
                    BrickDataArray[currRow, currCol].brickColor =
                        levelData.brickDataArray.rowArray[currRow].rowBricks[currCol].brickColor;
                    BrickDataArray[currRow, currCol].brickBonus =
                        levelData.brickDataArray.rowArray[currRow].rowBricks[currCol].brickBonus;
                    BrickDataArray[currRow, currCol].rowNumber =
                        levelData.brickDataArray.rowArray[currRow].rowBricks[currCol].rowNumber;
                    BrickDataArray[currRow, currCol].columnNumber =
                        levelData.brickDataArray.rowArray[currRow].rowBricks[currCol].columnNumber;
                }
            }

            MaxEnemies = levelData.maxEnemies;
            MinEnemyTime = levelData.minTimeBetweenEnemies;
            MaxEnemyTime = levelData.maxTimeBetweenEnemies;
            IsBossLevel = levelData.isBossLevel;
            LevelBossIndex = levelData.levelBossIndex;
            BackdropSceneIndex = levelData.levelBackdropSceneIndex;
            BackgroundMusicIndex = levelData.levelBackgroundMusicIndex;
            FileName = levelData.levelFileName;
            LevelName = levelData.levelName;
        }
    }
}
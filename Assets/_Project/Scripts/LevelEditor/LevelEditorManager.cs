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

        [FoldoutGroup("Events")] public UnityEvent<List<string>> LoadLevelListChangeEvent;
        // [FoldoutGroup("Events")] public UnityEvent DataClearedEvent;
        // [FoldoutGroup("Events")] public UnityEvent<BrickData[,]> BrickDataChangedEvent;
        [FoldoutGroup("Events")] public UnityEvent<BrickData> BrickUpdatedEvent;
        [FoldoutGroup("Events")] public UnityEvent<string> LevelDescChangedEvent;
        [FoldoutGroup("Events")] public UnityEvent<int> LevelBackgroundIndexChangedEvent;
        [FoldoutGroup("Events")] public UnityEvent<int> MaxEnemiesChangedEvent;
        [FoldoutGroup("Events")] public UnityEvent<float> MinEnemyTimeChangedEvent;
        [FoldoutGroup("Events")] public UnityEvent<float> MaxEnemyTimeChangedEvent;

        // Level Public properties
        public int MaxEnemies { get; set; } = 0;
        public float MinEnemyTime { get; set; } = 0.0f;
        public float MaxEnemyTime { get; set; } = 0.0f;
        public int BackgroundSpriteIndex { get; set; } = 0;
        public string FileName { get; set; }
        public string LevelDesc { get; set; }

        // Internal 2 dimension array of brick data
        private BrickData[,] _brickDataArray;
    
        /// <summary>
        /// Set up the Level Editor Manager
        /// </summary>
        private void Awake()
        {
            // Initialise internal array
            _brickDataArray = new BrickData[numberOfRows, numberOfBricksPerRow];
        }

        /// <summary>
        /// Initialise the component
        /// </summary>
        private void Start()
        {
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
                    _brickDataArray[currRow, currCol] =
                        new BrickData(BrickType.Normal, Color.white, BonusType.None, true, currRow, currCol);
                    BrickUpdatedEvent.Invoke(_brickDataArray[currRow, currCol]);
                }
            }

            LevelDesc = "";
            MaxEnemies = 0;
            MinEnemyTime = 0.0f;
            MaxEnemyTime = 0.0f;
            BackgroundSpriteIndex = 0;

            LevelDescChangedEvent.Invoke(LevelDesc);
            MaxEnemiesChangedEvent.Invoke(MaxEnemies);
            MinEnemyTimeChangedEvent.Invoke(MinEnemyTime);
            MaxEnemyTimeChangedEvent.Invoke(MaxEnemyTime);
            LevelBackgroundIndexChangedEvent.Invoke(BackgroundSpriteIndex);
        }

        /// <summary>
        /// Populate the drop down with current level list
        /// </summary>
        public void GetCurrentLevels(bool isCustomLevels)
        {
            List<string>levels = new List<string>();
            
            foreach(string fileName in LevelDataExt.GetLevelsNames(isCustomLevels))
            {
                levels.Add(Path.GetFileNameWithoutExtension(fileName));
            }
            LoadLevelListChangeEvent.Invoke(levels);
        }

        /// <summary>
        /// Update a brick with the current selection, returns a brick
        /// specification string
        /// </summary>
        /// <param name="brickData"></param>
        /// <returns></returns>
        public void UpdateBrick(BrickData brickData)
        {
            int row = brickData.RowNumber;
            int column = brickData.ColumnNumber;

            if (brickData.HasBrickBonusChanged)
            {
                _brickDataArray[row, column].BrickBonus = brickData.BrickBonus;
            }

            if (brickData.HasBrickTypeChanged)
            {
                _brickDataArray[row, column].BrickType = brickData.BrickType;
            }

            if (brickData.HasIsEmptySlotChanged)
            {
                _brickDataArray[row, column].IsEmptySlot = brickData.IsEmptySlot;
            }

            if (brickData.HasBrickColorChanged)
            {
                _brickDataArray[row, column].BrickColor = brickData.BrickColor;
            }
            BrickUpdatedEvent.Invoke(_brickDataArray[row, column]);
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
        /// Loads a level from the given named asset file
        /// </summary>
        /// <param name="levelFileName"></param>
        /// <param name="isCustomLevels"></param>
        public void LoadLevelByName(string levelFileName, bool isCustomLevels)
        {
            LevelDataExt levelData = LevelDataExt.LoadInstanceFromFile(levelFileName, isCustomLevels);
            if (levelData == null)
            {
                Debug.Log($"Error: Level data file not found at path {levelFileName}");
                return;
            }

            for (int currRow = 0; currRow < numberOfRows; currRow++)
            {
                for (int currCol = 0; currCol < numberOfBricksPerRow; currCol++)
                {
                    _brickDataArray[currRow, currCol].IsEmptySlot = levelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].IsEmptySlot;
                    _brickDataArray[currRow, currCol].BrickType = levelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].BrickType;
                    _brickDataArray[currRow, currCol].BrickColor = levelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].BrickColor;
                    _brickDataArray[currRow, currCol].BrickBonus = levelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].BrickBonus;
                    _brickDataArray[currRow, currCol].RowNumber =
                        levelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].RowNumber;
                    _brickDataArray[currRow, currCol].ColumnNumber =
                        levelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].ColumnNumber;
                    BrickUpdatedEvent.Invoke(_brickDataArray[currRow, currCol]);
                }
            }

            LevelDesc = levelData.levelName;
            MaxEnemies = levelData.maxEnemies;
            MinEnemyTime = levelData.minTimeBetweenEnemies;
            MaxEnemyTime = levelData.maxTimeBetweenEnemies;
            BackgroundSpriteIndex = levelData.levelBackgroundIndex;

            LevelDescChangedEvent.Invoke(LevelDesc);
            MaxEnemiesChangedEvent.Invoke(MaxEnemies);
            MinEnemyTimeChangedEvent.Invoke(MinEnemyTime);
            MaxEnemyTimeChangedEvent.Invoke(MaxEnemyTime);
            LevelBackgroundIndexChangedEvent.Invoke(BackgroundSpriteIndex);
        }

        /// <summary>
        /// Save the current level as a ScriptableObject instance
        /// </summary>
        /// <param name="levelName"></param>
        /// <param name="isCustomLevel"></param>
        public void SaveLevelAsNew(string levelName, bool isCustomLevel)
        {
            // LevelData newLevelData = ScriptableObject.CreateInstance<LevelData>();
            LevelDataExt newLevelData = new LevelDataExt();
            newLevelData.Init();

            newLevelData.levelName = LevelDesc;
            newLevelData.levelBackgroundIndex = BackgroundSpriteIndex;
            newLevelData.maxEnemies = MaxEnemies;
            newLevelData.minTimeBetweenEnemies = MinEnemyTime;
            newLevelData.maxTimeBetweenEnemies = MaxEnemyTime;

            for (int currRow = 0; currRow < numberOfRows; currRow++)
            {
                for (int currCol = 0; currCol < numberOfBricksPerRow; currCol++)
                {
                    newLevelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].IsEmptySlot = _brickDataArray[currRow, currCol].IsEmptySlot;
                    newLevelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].BrickType = _brickDataArray[currRow, currCol].BrickType;
                    newLevelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].BrickColor = _brickDataArray[currRow, currCol].BrickColor;
                    newLevelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].BrickBonus = _brickDataArray[currRow, currCol].BrickBonus;
                    newLevelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].RowNumber = _brickDataArray[currRow, currCol].RowNumber;
                    newLevelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].ColumnNumber = _brickDataArray[currRow, currCol].ColumnNumber;
                }
            }
            newLevelData.SaveInstanceToFile(levelName, isCustomLevel);
            GetCurrentLevels(isCustomLevel);
        }
    }
}

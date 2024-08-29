using System.Collections.Generic;
using System.Linq;
using DaftAppleGames.RetroRacketRevolution.Game;
using DaftAppleGames.RetroRacketRevolution.Bricks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution.Levels
{
    public class LevelLoader : MonoBehaviour
    {
        [BoxGroup("Managers")] public BrickManager brickManager;
        [BoxGroup("Level Layout")] public int numberOfRows = 12;
        [BoxGroup("Level Layout")] public int numberOfBricksPerRow = 15;
        [BoxGroup("Level Layout")] public GameObject levelRoot;
        [BoxGroup("Level Layout")] public Rect playAreaRect;
        [BoxGroup("Level Layout")] public float brickWidthBuffer = 0.1f;
        [BoxGroup("Level Layout")] public float brickHeightBuffer = 0.1f;
        [BoxGroup("Level Graphics")] public SpriteRenderer backgroundSpriteRenderer;
        [BoxGroup("Objects")] public Vector2 brickScale = new Vector2(5.2f, 2.7f);
        [BoxGroup("Objects")] public Vector2 disruptorScale = new Vector2(1.0f, 1.0f);
        [BoxGroup("Levels")] public List<LevelLoadEntry> levelFiles;
        [BoxGroup("Game Data")] public GameData gameData;
        [FoldoutGroup("Background Sprites")] public LevelBackgroundSprites backgroundSprites;
        [FoldoutGroup("Events")] public UnityEvent<LevelDataExt> LevelLoadedEvent;
        [FoldoutGroup("Events")] public UnityEvent<int> LevelLoadedMusicEvent;

        private float _brickWidthScale = 0.0f;
        private float _brickHeightScale = 0.0f;

        public int CurrentLevel { get; private set; }

        public class LevelLoadEntry
        {
            public string LevelFileName;
            public bool IsCustomLevel;

            public LevelLoadEntry(string levelFileName, bool isCustomLevel)
            {
                LevelFileName = levelFileName;
                IsCustomLevel = isCustomLevel;
            }
        }

        /// <summary>
        /// Init the component
        /// </summary>
        private void Awake()
        {
            List<string> ogLevelFiles = LevelDataExt.GetLevelsNames(false);
            List<string> customLevelFiles = LevelDataExt.GetLevelsNames(true);

            levelFiles = GetLevelLoadEntries(gameData.levelSelect);
        }

        /// <summary>
        /// Get list of levels to load
        /// </summary>
        /// <param name="levelSelect"></param>
        /// <returns></returns>
        private List<LevelLoadEntry> GetLevelLoadEntries(LevelSelect levelSelect)
        {
            List<string> ogLevelFiles = LevelDataExt.GetLevelsNames(false);
            List<string> customLevelFiles = LevelDataExt.GetLevelsNames(true);

            List<LevelLoadEntry> levelLoadEntries = new List<LevelLoadEntry>();

            // Add Original levels
            if (levelSelect == LevelSelect.Original || levelSelect == LevelSelect.OgPlusCustom)
            {
                foreach (string levelFileName in ogLevelFiles)
                {
                    levelLoadEntries.Add(new LevelLoadEntry(levelFileName, false));
                }
            }

            // Add Custom levels
            if (levelSelect == LevelSelect.Custom || levelSelect == LevelSelect.OgPlusCustom)
            {
                foreach (string levelFileName in customLevelFiles)
                {
                    levelLoadEntries.Add(new LevelLoadEntry(levelFileName, true));
                }
            }

            return levelLoadEntries;
        }

        /// <summary>
        /// Start up components
        /// </summary>
        private void Start()
        {
            CurrentLevel = 0; 
            LoadNextLevel();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.0f, 1.0f, 0.0f);
            DrawRect(playAreaRect);
        }

        /// <summary>
        /// Draw a rect
        /// </summary>
        /// <param name="rect"></param>
        private void DrawRect(Rect rect)
        {
            Vector2 newPosition = new Vector2(rect.position.x, rect.position.y);
            rect.position = newPosition;
            // Vector3 center = new Vector3 (rect.x + (rect.width / 2.0f), (rect.y + rect.height / 2.0f), 0.01f);
            // Debug.Log($"Rect at: {center.x}, {center.y}");
            Gizmos.DrawWireCube(new Vector3(rect.center.x, rect.center.y, 0.01f), new Vector3(rect.size.x, rect.size.y, 0.01f));
        }
#endif

        /// <summary>
        /// Returns true of on the last level
        /// </summary>
        /// <returns></returns>
        public bool IsLastLevel()
        {
            return (CurrentLevel == levelFiles.Count);
        }

        /// <summary>
        /// Loads the next level
        /// </summary>
        public bool LoadNextLevel()
        {
            if (CurrentLevel == levelFiles.Count)
            {
                return false;
            }
            LoadLevel(levelFiles[CurrentLevel]);
            CurrentLevel++;
            return true;
        }

        /// <summary>
        /// Loads the given level file
        /// </summary>
        /// <param name="levelLoadEntry"></param>
        private void LoadLevel(LevelLoadEntry levelLoadEntry)
        {
            LevelDataExt levelData = LevelDataExt.LoadInstanceFromFile(levelLoadEntry.LevelFileName, levelLoadEntry.IsCustomLevel);
            if (levelData == null)
            {
                Debug.Log($"An error occurred loading: {levelLoadEntry.LevelFileName} with IsCustomLevel set to: {levelLoadEntry.IsCustomLevel}");
                return;
            }
            LoadLevelData(levelData);
        }

        /// <summary>
        /// Get Sprite by name
        /// </summary>
        /// <param name="textureName"></param>
        public Sprite GetBackgroundSpriteByName(string spriteName)
        {
            foreach (Sprite sprite in backgroundSprites.BackgroundSprites)
            {
                if (sprite.name == spriteName)
                {
                    return sprite;
                }
            }
            return backgroundSprites.BackgroundSprites[0];
        }

        /// <summary>
        /// Get Sprite by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Sprite GetBackgroundSpriteByIndex(int index)
        {
            return backgroundSprites.BackgroundSprites[index];
        }

        /// <summary>
        /// Loads the specified level data load file
        /// </summary>
        /// <param name="levelData"></param>
        private void LoadLevelData(LevelDataExt levelData)
        {
            float brickWidth = (playAreaRect.width / numberOfBricksPerRow) + brickWidthBuffer;
            float brickHeight = System.Math.Abs((playAreaRect.height / numberOfRows) + brickHeightBuffer);

            Vector2 startingPosition = new Vector2(playAreaRect.x + (brickWidth / 2), playAreaRect.y -(brickHeight / 2));

            float currBrickHor = startingPosition.x;
            float currBrickVert = startingPosition.y;


            // Load background
            backgroundSpriteRenderer.sprite = GetBackgroundSpriteByIndex(levelData.levelBackgroundIndex);

            // Alternate sorting groups to allow glint sprite mask
            bool isMainSortingGroup = true;

            // Load bricks
            for (int currRow = 0; currRow < numberOfRows; currRow++) 
            {
                for (int currCol = 0; currCol < numberOfBricksPerRow; currCol++)
                {
                    BrickData currLoadBrickData = levelData.BrickDataArray.RowArray[currRow].RowBricks[currCol];
                    
                    // If this is an "empty" brick, then skip
                    if (!currLoadBrickData.IsEmptySlot)
                    {
                        // Handle disruptor
                        if (currLoadBrickData.BrickType == BrickType.DisruptorIn ||
                            currLoadBrickData.BrickType == BrickType.DisruptorOut ||
                            currLoadBrickData.BrickType == BrickType.DisruptorBoth)
                        {
                            Disruptor newDisruptor =
                                brickManager.SpawnDisruptor(currLoadBrickData.BrickType, currRow, currCol);
                            newDisruptor.gameObject.transform.localPosition = new Vector2(currBrickHor, currBrickVert);
                            newDisruptor.gameObject.transform.localScale = disruptorScale;
                        }
                        else
                        {
                            // Otherwise, instantiate, position and configure the brick
                            GameObject newBrickGameObject = brickManager.SpawnBrick(currLoadBrickData.BrickType,
                                currLoadBrickData.BrickColor, currLoadBrickData.BrickBonus, currRow, currCol, isMainSortingGroup).gameObject;
                            isMainSortingGroup = !isMainSortingGroup;

                            // Calculate brick width scale, if we haven't calculated it yet
                            if (_brickWidthScale == 0.0f || _brickHeightScale == 0.0f)
                            {
                                SpriteRenderer spriteRenderer = newBrickGameObject.GetComponentInChildren<SpriteRenderer>();
                                Sprite sprite = spriteRenderer.sprite;
                                _brickWidthScale = sprite.rect.width / brickWidth;
                                _brickHeightScale = sprite.rect.height / brickHeight;
                            }
                            // newBrickGameObject.transform.parent = newBricksGameObject.transform;
                            newBrickGameObject.transform.localPosition = new Vector2(currBrickHor, currBrickVert);
                            newBrickGameObject.transform.localScale = brickScale;
                        }
                    }
                    currBrickHor += brickWidth;
                }
                currBrickHor = startingPosition.x;
                currBrickVert -= brickHeight;
            }
            LevelLoadedEvent.Invoke(levelData);
            LevelLoadedMusicEvent.Invoke(levelData.levelBackgroundMusicIndex);
        }
    }
}
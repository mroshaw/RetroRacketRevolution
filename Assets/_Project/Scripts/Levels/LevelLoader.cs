using System.Collections.Generic;
using System.Linq;
using DaftAppleGames.RetroRacketRevolution.Game;
using DaftApplesGames.RetroRacketRevolution.Bricks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftApplesGames.RetroRacketRevolution.Levels
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
        [BoxGroup("Levels")] public List<string> levelFiles;
        [BoxGroup("Game Data")] public GameData gameData;
        [FoldoutGroup("Background Sprites")] public LevelBackgroundSprites backgroundSprites;
        [FoldoutGroup("Events")] public UnityEvent<LevelDataExt> LevelLoadedEvent;
        
        private float _brickWidthScale = 0.0f;
        private float _brickHeightScale = 0.0f;

        public int CurrentLevel { get; private set; }

        /// <summary>
        /// Init the component
        /// </summary>
        private void Awake()
        {
            List<string> ogLevelFiles = LevelDataExt.GetLevelsNames(false);
            List<string> customLevelFiles = LevelDataExt.GetLevelsNames(true);

            switch (gameData.levelSelect)
            {
                case LevelSelect.Original:
                    levelFiles = ogLevelFiles;
                    break;
                case LevelSelect.Custom:
                    levelFiles = customLevelFiles;
                    break;
                case LevelSelect.OgPlusCustom:
                    levelFiles = (List<string>)ogLevelFiles.Concat(customLevelFiles);
                    break;
                case LevelSelect.CustomPlusOg:
                    levelFiles = (List<string>)customLevelFiles.Concat(ogLevelFiles);
                    break;
            }
        }

        /// <summary>
        /// Start up components
        /// </summary>
        private void Start()
        {
            // LoadLevel(levels[0]);
            CurrentLevel = 0; 
            LoadNextLevelPlease();
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
        public bool LoadNextLevelPlease()
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
        /// <param name="levelFileName"></param>
        private void LoadLevel(string levelFileName)
        {
            LevelDataExt levelData = null;

            switch (gameData.levelSelect)
            {
                case LevelSelect.Custom:
                    levelData = LevelDataExt.LoadInstanceFromFile(levelFileName, true);
                    break;

                case LevelSelect.Original:
                    levelData = LevelDataExt.LoadInstanceFromFile(levelFileName, false);
                    break;
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
        }
    }
}

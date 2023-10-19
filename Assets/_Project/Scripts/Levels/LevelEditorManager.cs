using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using DaftApplesGames.RetroRacketRevolution.Bonuses;
using DaftApplesGames.RetroRacketRevolution.Bricks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Runtime.ConstrainedExecution;

namespace DaftApplesGames.RetroRacketRevolution.Levels
{
    public class LevelEditorManager : MonoBehaviour
    {
        [BoxGroup("UI")] public TMP_InputField fileNameText;
        [BoxGroup("UI")] public TMP_InputField levelDescription;
        [BoxGroup("UI")] public GameObject grid;
        [BoxGroup("UI")] public TMP_Dropdown loadLevelDropDown;
        [BoxGroup("UI")] public Button CurrentColourButton;
        [BoxGroup("UI")] public TMP_Dropdown backGroundSpriteDropDown;
        [BoxGroup("UI")] public TMP_Dropdown brickTypeDropDown;
        [BoxGroup("UI")] public TMP_Dropdown brickBonusDropDown;
        [BoxGroup("UI")] public TMP_InputField maxEnemiesText;
        [BoxGroup("UI")] public TMP_InputField minTimeBetweenEnemiesText;
        [BoxGroup("UI")] public TMP_InputField maxTimeBetweenEnemiesText;
        [BoxGroup("Alerts")] public TextMeshProUGUI alertText;
        [BoxGroup("Alerts")] public float alertFadeTime = 2.0f;
        [BoxGroup("Alerts")] public float alertVisibleTime = 0.0f;
        [BoxGroup("Alerts")] public AudioClip alertAudioClip;
        [BoxGroup("Alerts")] public AudioClip errorAudioClip;
        [FoldoutGroup("Background Sprites")] public LevelBackgroundSprites backgroundSprites;
        [BoxGroup("Level Layout")] public int numberOfRows = 12;
        [BoxGroup("Level Layout")] public int numberOfBricksPerRow = 15;
        [BoxGroup("Disruptor")] public int disruptorRows = 3;
        [BoxGroup("Disruptor")] public int disruptorColumns = 3;
        

        private bool _stampBonus = true;
        private bool _stampType = true;
        private bool _stampColour = true;
        private bool _stampIsEmpty = true;

        // Variables for notification fader
        private Color _visibleColor;
        private Color _hiddenColor;

        private int _disruptorColsOffset;
        private int _disruptorRowsOffset;

        private AudioSource _audioSource;

        public static LevelEditorManager Instance { get; private set; }

        // Internal 2 dimension array of brick data
        public BrickData[,] BrickDataArray;

        public BrickType CurrBrickType = BrickType.Normal;
        public Color CurrColor = Color.yellow;
        public bool CurrIsEmptySlot = false;
        public BonusType CurrBrickBonus = BonusType.None;

        // Where level details will be stored
        public const string LevelAssetPath = "Assets/_Project/LevelData/";

        /// <summary>
        /// Set up the Level Editor Manager
        /// </summary>
        private void Awake()
        {
            // Maintain singleton
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;

                // Initialise internal array
                BrickDataArray = new BrickData[numberOfRows, numberOfBricksPerRow];
                _audioSource = GetComponent<AudioSource>();
                Color textColor = alertText.color;
                _visibleColor = new Color(textColor.r, textColor.g, textColor.b, 1);
                _hiddenColor = new Color(textColor.r, textColor.g, textColor.b, 0);

                _disruptorColsOffset = (disruptorColumns - 1) / 2;
                _disruptorRowsOffset = (disruptorRows - 1) / 2;
            }
        }

        /// <summary>
        /// Initialise the component
        /// </summary>
        private void Start()
        {
            Clear();
            PopulateCurrentLevels();
            PopulateBackgroundSprites();
            PopulateBrickTypeDropDown();
            PopulateBonusTypeDropDown();
        }

        /// <summary>
        /// Handle a change to the Bonus stamp check box
        /// </summary>
        /// <param name="state"></param>
        public void SetStampBonus(bool state)
        {
            _stampBonus = state;
        }

        /// <summary>
        /// Handle a change to the Type stamp check box
        /// </summary>
        /// <param name="state"></param>
        public void SetStampType(bool state)
        {
            _stampType = state;
        }

        /// <summary>
        /// Handle a change to the Colour stamp check box
        /// </summary>
        /// <param name="state"></param>
        public void SetStampColour(bool state)
        {
            _stampColour = state;
        }

        /// <summary>
        /// Handle a change to the IsEmpty stamp checkbox
        /// </summary>
        /// <param name="state"></param>
        public void SetStampIsEmpty(bool state)
        {
            _stampIsEmpty = state;
        }

        /// <summary>
        /// Refreshes the data set with empty bricks
        /// </summary>
        public void Clear()
        {
            for (int currRow = 0; currRow < numberOfRows; currRow++)
            {
                for (int currCol = 0; currCol < numberOfBricksPerRow; currCol++)
                {
                    BrickDataArray[currRow, currCol] =
                        new BrickData(BrickType.Normal, Color.white, BonusType.None, true, currRow, currCol);
                }
            }
            CurrIsEmptySlot = false;
            fileNameText.text = "";
            levelDescription.text = "";
            RefreshUi();
            Alert("Level data cleared", false);
        }

        /// <summary>
        /// Display an alert
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isError"></param>
        private void Alert(string message, bool isError)
        {
            if (isError)
            {
                _audioSource.PlayOneShot(errorAudioClip);
            }
            else
            {
                _audioSource.PlayOneShot(alertAudioClip);
            }
            StartCoroutine(NotifyFade(message));
        }

        /// <summary>
        /// Populate the drop down with current level list
        /// </summary>
        private void PopulateCurrentLevels()
        {
            List<TMP_Dropdown.OptionData>options = new List<TMP_Dropdown.OptionData>();
            
            // Add blank entry
            /*
            options.Add(new TMP_Dropdown.OptionData(""));
            foreach (string file in Directory.EnumerateFiles($"{Application.dataPath}/_Project/LevelData", "*.asset"))
            {
                options.Add(new TMP_Dropdown.OptionData(Path.GetFileNameWithoutExtension(file)));
            }
            */
            foreach(string fileName in LevelDataExt.GetLevelsNames())
            {
                options.Add(new TMP_Dropdown.OptionData(Path.GetFileNameWithoutExtension(fileName)));
            }
            loadLevelDropDown.options = options;
        }

        /// <summary>
        /// Populates the available background sprites
        /// </summary>
        private void PopulateBackgroundSprites()
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            foreach (Sprite sprite in backgroundSprites.BackgroundSprites)
            {
                options.Add(new TMP_Dropdown.OptionData(sprite.name, sprite));
            }

            backGroundSpriteDropDown.options = options;
        }

        /// <summary>
        /// Populate the Brick Type dropdown
        /// </summary>
        private void PopulateBrickTypeDropDown()
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            foreach (string brickTypeString in Enum.GetNames(typeof(BrickType)))
            {
                options.Add(new TMP_Dropdown.OptionData(brickTypeString));
            }

            brickTypeDropDown.options = options;
        }

        /// <summary>
        /// Populate the Bonus Type dropdown
        /// </summary>
        private void PopulateBonusTypeDropDown()
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            foreach (string bonusTypeString in Enum.GetNames(typeof(BonusType)))
            {
                options.Add(new TMP_Dropdown.OptionData(bonusTypeString));
            }

            brickBonusDropDown.options = options;
        }

        /// <summary>
        /// Handles a change to the Level dropdown
        /// </summary>
        /// <param name="levelIndex"></param>
        public void OnLevelChanged(int levelIndex)
        {
            Debug.Log($"Level Name changed...{loadLevelDropDown.options[levelIndex].text}");
            fileNameText.text = loadLevelDropDown.options[levelIndex].text;
        }

        /// <summary>
        /// Check the area around the brick for an existing disruptor
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private bool CanPlaceDisruptor(int row, int column)
        {
            // Disruptor needs space, so check it will fit
            // if (row + _disruptorRowsOffset >= numberOfRows || row < _disruptorRowsOffset || column + _disruptorColsOffset >= numberOfBricksPerRow || column < _disruptorColsOffset)
            if (row < _disruptorRowsOffset || column + _disruptorColsOffset >= numberOfBricksPerRow || column < _disruptorColsOffset)
            {
                Alert($"Cannot place a disruptor here. Requires {disruptorColumns} x {disruptorRows} brick spaces to place.", true);
                return false;
            }

            // Check there aren't disruptor bricks overlapping
            for (int disCol = -_disruptorColsOffset; disCol < disruptorColumns - _disruptorColsOffset; disCol++)
            {
                for (int disRow = -_disruptorRowsOffset; disRow < disruptorRows - _disruptorRowsOffset; disRow++)
                {
                    if (row + disRow < 0)
                    {
                        continue;
                    }
                    if (BrickDataArray[row + disRow, column + disCol].IsDisruptor())
                    {
                        Alert("Cannot overwrite existing disruptor!", true);
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Updates the bricks around a new disruptor, or clears them down
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="disruptorType"></param>
        /// <param name="clear"></param>
        private void UpdateDisruptor(int row, int column, BrickType disruptorType, bool clear)
        {
            // The disruptor takes x * y spaces, so we clear the brick to the right and below
            for (int disCol = -_disruptorColsOffset; disCol < disruptorColumns - _disruptorColsOffset; disCol++)
            {
                for (int disRow = -_disruptorRowsOffset; disRow < disruptorRows - _disruptorRowsOffset; disRow++)
                {
                    // We can get away with overlapping the bottom of the grid
                    if (row + disRow < 0)
                    {
                        continue;
                    }

                    if (clear)
                    {
                        BrickDataArray[row + disRow, column + disCol].BrickType = BrickType.Normal;
                        BrickDataArray[row + disRow, column + disCol].IsEmptySlot = true;
                    }
                    else
                    {
                        BrickDataArray[row + disRow, column + disCol].BrickType = CurrBrickType;
                        BrickDataArray[row + disRow, column + disCol].IsEmptySlot = true;
                    }
                }
            }

            // Now set the disruptor in the middle
            if (clear)
            {
                BrickDataArray[row, column].BrickType = BrickType.Normal;
                BrickDataArray[row, column].IsEmptySlot = true;
            }
            else
            {
                BrickDataArray[row, column].BrickType = CurrBrickType;
                BrickDataArray[row, column].IsEmptySlot = false;
            }
            RefreshUi();
        }

        /// <summary>
        /// Update a brick with the current selection, returns a brick
        /// specification string
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public BrickData UpdateBrick(int row, int column)
        {
            BrickData selectedBrick = BrickDataArray[row, column];

            // Deal with the disruptor brick

            // Check if we're updating a disruptor or clearing down
            if (selectedBrick.IsDisruptor())
            {
                if (selectedBrick.IsEmptySlot && CurrIsEmptySlot == true)
                {
                    Alert("Set IsEmpty on main disruptor brick first!", true);
                    return null;
                }
                else
                {
                    // Clear disruptor
                    UpdateDisruptor(row, column, BrickType.Normal, true);
                }
            }

            // We're adding a new disruptor
            if (CurrBrickType == BrickType.DisruptorIn || CurrBrickType == BrickType.DisruptorOut || CurrBrickType == BrickType.DisruptorBoth)
            {
                if (!CanPlaceDisruptor(row, column))
                {
                    return null;
                }
                UpdateDisruptor(row, column, CurrBrickType, false);
                return null;
            }

            // Apply Color, if appropriate
            Color updateColour;
            if (_stampColour)
            {
                updateColour = CurrColor;
            }
            else
            {
                updateColour = BrickDataArray[row, column].BrickColor;
            }

            // Apply IsEmpty state, if appropriate
            bool updateIsEmpty;
            if (_stampIsEmpty)
            {
                updateIsEmpty = CurrIsEmptySlot;
                if (CurrIsEmptySlot)
                {
                    updateColour = Color.white;
                }
            }
            else
            {
                updateIsEmpty = BrickDataArray[row, column].IsEmptySlot;
            }

            // Apply Bonus, if appropriate
            BonusType updateBonus;
            if (_stampBonus)
            {
                updateBonus = CurrBrickBonus;
            }
            else
            {
                updateBonus = BrickDataArray[row, column].BrickBonus;
            }

            // Apply Type, if appropriate
            BrickType updateType;
            if (_stampType)
            {
                updateType = CurrBrickType;
            }
            else
            {
                updateType = BrickDataArray[row, column].BrickType;
            }
            // Debug.Log($"Updating brick: {row}, {column}");
            if (CurrIsEmptySlot)
            {
                BrickDataArray[row, column].BrickColor = Color.white;
            }
            else
            {
                BrickDataArray[row, column].BrickColor = updateColour;
            }
            BrickDataArray[row, column].BrickType = updateType;
            BrickDataArray[row, column].IsEmptySlot = updateIsEmpty;
            BrickDataArray[row, column].BrickBonus = updateBonus;
            return BrickDataArray[row, column];
        }

        /// <summary>
        /// Sets the IsEmpty value based on selected toggle
        /// </summary>
        /// <param name="state"></param>
        public void SetIsEmptySlot(bool state)
        {
            Debug.Log("Setting slot state to: {state}");
            CurrIsEmptySlot = state;
        }

        /// <summary>
        /// Sets the current BrickType based on drop down
        /// </summary>
        /// <param name="typeIndex"></param>
        public void SetBrickType(int typeIndex)
        {
            CurrBrickType = (BrickType)typeIndex;
        }

        /// <summary>
        /// Set the current BrickBonus based on drop down
        /// </summary>
        /// <param name="bonusIndex"></param>
        public void SetBrickBonus(int bonusIndex)
        {
            CurrBrickBonus = (BonusType)bonusIndex;
        }

        /// <summary>
        /// Return to Main Menu scene
        /// </summary>
        public void MainMenu()
        {
            SceneManager.LoadScene("MainMenuScene");
        }

        /// <summary>
        /// Loads the level selected
        /// </summary>
        public void LoadLevel()
        {
            string levelName = loadLevelDropDown.options[loadLevelDropDown.value].text;

            if (string.IsNullOrEmpty(levelName))
            {
                Alert("Please select a level to load!", true);
                return;
            }

            LoadLevelByName(levelName);
            Alert($"Loaded {levelName} successfully.", false);
        }

        /// <summary>
        /// Loads a level from the given named asset file
        /// </summary>
        /// <param name="levelFileName"></param>
        public void LoadLevelByName(string levelFileName)
        {
#if UNITY_EDITOR
            // LevelData levelData = (LevelData)AssetDatabase.LoadAssetAtPath($"{LevelAssetPath}/{levelName}.asset", typeof(LevelData));
            LevelDataExt levelData = LevelDataExt.LoadInstanceFromFile(levelFileName);
#else
            LevelDataExt levelData = LevelDataExt.LoadInstanceFromFile(levelFileName);
#endif
            if (levelData == null)
            {
                Debug.Log($"Error: Asset not found at path {LevelAssetPath}/{levelFileName}.asset");
                return;
            }

            for (int currRow = 0; currRow < numberOfRows; currRow++)
            {
                for (int currCol = 0; currCol < numberOfBricksPerRow; currCol++)
                {
                    Debug.Log($"Setting: {currRow}, {currCol}");
                    BrickDataArray[currRow, currCol].IsEmptySlot = levelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].IsEmptySlot;
                    BrickDataArray[currRow, currCol].BrickType = levelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].BrickType;
                    BrickDataArray[currRow, currCol].BrickColor = levelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].BrickColor;
                    BrickDataArray[currRow, currCol].BrickBonus = levelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].BrickBonus;
                    BrickDataArray[currRow, currCol].RowNumber =
                        levelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].RowNumber;
                    BrickDataArray[currRow, currCol].ColumnNumber =
                        levelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].ColumnNumber;
                }
            }

            levelDescription.text = levelData.levelName;
            backGroundSpriteDropDown.value = levelData.levelBackgroundIndex;
            fileNameText.text = levelFileName;
            maxEnemiesText.text = levelData.maxEnemies.ToString();
            minTimeBetweenEnemiesText.text = levelData.minTimeBetweenEnemies.ToString();
            maxTimeBetweenEnemiesText.text = levelData.maxTimeBetweenEnemies.ToString();
            RefreshUi();
        }

        /// <summary>
        /// Saves the current level
        /// </summary>
        public void SaveLevel()
        {
            string levelName = fileNameText.text;
            if (string.IsNullOrEmpty(levelName))
            {
                Alert("Please enter a filename before saving!", true);
                return;
            }

            if (string.IsNullOrEmpty(levelDescription.text))
            {
                Alert("Please enter a level description before saving!", true);
                return;
            }

            SaveLevelAsNew(levelName);
            Alert($"Saved {levelName} successfully.", false);
        }

        /// <summary>
        /// Save the current level as a ScriptableObject instance
        /// </summary>
        /// <param name="levelName"></param>
        public void SaveLevelAsNew(string levelName)
        {
            if (string.IsNullOrEmpty(levelName))
            {
                return;
            }

            // LevelData newLevelData = ScriptableObject.CreateInstance<LevelData>();
            LevelDataExt newLevelData = new LevelDataExt();
            newLevelData.Init();

            newLevelData.levelName = levelDescription.text;
            newLevelData.levelBackgroundIndex = backGroundSpriteDropDown.value;
            newLevelData.maxEnemies = int.Parse(maxEnemiesText.text);
            newLevelData.minTimeBetweenEnemies = float.Parse(minTimeBetweenEnemiesText.text);
            newLevelData.maxTimeBetweenEnemies = float.Parse(maxTimeBetweenEnemiesText.text);

            for (int currRow = 0; currRow < numberOfRows; currRow++)
            {
                for (int currCol = 0; currCol < numberOfBricksPerRow; currCol++)
                {
                    Debug.Log($"Setting: {currCol}, {currRow}");
                    newLevelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].IsEmptySlot = BrickDataArray[currRow, currCol].IsEmptySlot;
                    newLevelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].BrickType = BrickDataArray[currRow, currCol].BrickType;
                    newLevelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].BrickColor = BrickDataArray[currRow, currCol].BrickColor;
                    newLevelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].BrickBonus = BrickDataArray[currRow, currCol].BrickBonus;
                    newLevelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].RowNumber = BrickDataArray[currRow, currCol].RowNumber;
                    newLevelData.BrickDataArray.RowArray[currRow].RowBricks[currCol].ColumnNumber = BrickDataArray[currRow, currCol].ColumnNumber;
                }
            }

#if UNITY_EDITOR
            newLevelData.SaveInstanceToFile(levelName);

#else
            newLevelData.SaveInstanceToFile(levelName);

#endif
            PopulateCurrentLevels();
        }

        /// <summary>
        /// Refresh the Level Editor UI
        /// </summary>
        public void RefreshUi()
        {
            Button[] allButtons = grid.GetComponentsInChildren<Button>(false);

            // Debug.Log($"Found {allButtons.Length} buttons in grid");

            foreach (Button button in allButtons)
            {
                if (!button.interactable)
                {
                    continue;
                }
                TextMeshProUGUI labelText = button.GetComponentInChildren<TextMeshProUGUI>();
                string[] coords = labelText.text.Split(",");

                int x = Int32.Parse(coords[0]);
                int y = Int32.Parse(coords[1]);

                Debug.Log($"Updating: {x}, {y}");

                bool isEmpty = BrickDataArray[x, y].IsEmptySlot;

                BrickType brickType = BrickDataArray[x, y].BrickType;

                if (brickType == BrickType.DisruptorBoth || brickType == BrickType.DisruptorIn ||
                    brickType == BrickType.DisruptorOut)
                {
                    button.GetComponentInChildren<Image>().color = Color.grey;
                }
                else if (isEmpty)
                {
                    button.GetComponentInChildren<Image>().color = Color.white;
                }
                else
                {
                    button.GetComponentInChildren<Image>().color = BrickDataArray[x, y].BrickColor;
                }

                string newLabelText = BrickDataArray[x, y].Label;
                labelText.text = newLabelText;
            }
        }

        /// <summary>
        /// Show the current notification queue, fade in and out
        /// </summary>
        /// <returns></returns>
        private IEnumerator NotifyFade(string message)
        {
            alertText.color = _hiddenColor;
            alertText.text = message;

            // Fade text in
            float time = 0;
            while (time < alertFadeTime)
            {
                alertText.color = Color.Lerp(_hiddenColor, _visibleColor, time / alertFadeTime);
                time += Time.deltaTime;
                yield return null;
            }
            alertText.color = _visibleColor;

            // Wait
            yield return new WaitForSecondsRealtime(alertVisibleTime);

            // Fade out
            time = 0;
            while (time < alertFadeTime)
            {
                alertText.color = Color.Lerp(_visibleColor, _hiddenColor, time / alertFadeTime);
                time += Time.deltaTime;
                yield return null;
            }
            alertText.color = _hiddenColor;
            alertText.text = "";
        }
    }
}

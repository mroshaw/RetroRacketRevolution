using DaftAppleGames.RetroRacketRevolution.Audio;
using DaftAppleGames.RetroRacketRevolution.Bonuses;
using DaftAppleGames.RetroRacketRevolution.Bricks;
using DaftAppleGames.RetroRacketRevolution.Enemies;
using DaftAppleGames.RetroRacketRevolution.Levels;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DaftAppleGames.RetroRacketRevolution.LevelEditor
{
    public class LevelEditorWindow : MonoBehaviour
    {
        [BoxGroup("UI - Brick Grid")] [SerializeField] private GameObject grid;
        [BoxGroup("UI - Brick Grid")] [SerializeField] private Image gridBackgroundImage;
        [BoxGroup("UI - Stamp")] [SerializeField] private Toggle stampBonusToggle;
        [BoxGroup("UI - Stamp")] [SerializeField] private Toggle stampTypeToggle;
        [BoxGroup("UI - Stamp")] [SerializeField] private Toggle stampColorToggle;
        [BoxGroup("UI - Stamp")] [SerializeField] private Toggle stampIsEmptyToggle;
        [BoxGroup("UI - Brick")] [SerializeField] private TMP_Dropdown brickTypeDropDown;
        [BoxGroup("UI - Brick")] [SerializeField] private TMP_Dropdown brickBonusDropDown;
        [BoxGroup("UI - Brick")] [SerializeField] private Toggle brickIsEmptyToggle;
        [BoxGroup("UI - Brick")] [SerializeField] private Button mainColorButton;
        [BoxGroup("UI - Level")] [SerializeField] private TMP_InputField levelFileNameText;
        [BoxGroup("UI - Level")] [SerializeField] private TMP_InputField levelNameText;
        [BoxGroup("UI - Level")] [SerializeField] private TMP_Dropdown loadLevelDropDown;
        [BoxGroup("UI - Level")] [SerializeField] private TMP_Dropdown backdropScenesDropDown;
        [BoxGroup("UI - Level")] [SerializeField] private TMP_Dropdown backGroundMusicDropDown;
        [BoxGroup("UI - Level")] [SerializeField] private TMP_InputField maxEnemiesText;
        [BoxGroup("UI - Level")] [SerializeField] private TMP_InputField minTimeBetweenEnemiesText;
        [BoxGroup("UI - Level")] [SerializeField] private TMP_InputField maxTimeBetweenEnemiesText;
        [BoxGroup("UI - Level")] [SerializeField] private Toggle isCustomLevelsToggle;
        [BoxGroup("UI - Level")] [SerializeField] private Toggle levelIsBossLevelToggle;
        [BoxGroup("UI - Level")] [SerializeField] private TMP_Dropdown levelBossDropdown;
        [BoxGroup("UI - Other")] [SerializeField] private ConfirmWindow deleteWindow;
        [BoxGroup("UI - Other")] [SerializeField] private AlertText alertText;
        [BoxGroup("UI - Other")] [SerializeField] private string mainMenuScene;

        [BoxGroup("Data")] [SerializeField] private BrickTypeData brickTypeData;
        [BoxGroup("Data")] [SerializeField] private BonusData bonusData;
        [BoxGroup("Data")] [SerializeField] private LevelBackdropScenes backdropScenes;
        [BoxGroup("Data")] [SerializeField] private PlayList backgroundMusicData;
        [BoxGroup("Data")] [SerializeField] private EnemiesData levelBossData;

        [FoldoutGroup("Events - Brick Grid")] public UnityEvent<BrickData> brickGridClickedEvent;
        [FoldoutGroup("Events - Brick Grid")] public UnityEvent<BrickData> brickUpdatedEvent;

        [FoldoutGroup("Events - Other")] public UnityEvent levelShareClickedEvent;
        [FoldoutGroup("Events - Other")] public UnityEvent levelAddClickedEvent;

        private BrickButton[] _allBrickButtons;

        private LevelEditorManager _levelEditorManager;

        /// <summary>
        /// Initialise this component
        /// </summary>   
        private void Awake()
        {
            _levelEditorManager = GetComponent<LevelEditorManager>();
        }

        /// <summary>
        /// Populate controls
        /// </summary>
        private void Start()
        {
            // Enable switching between custom and OG levels in Editor only
#if UNITY_EDITOR
            isCustomLevelsToggle.gameObject.SetActive(true);
            isCustomLevelsToggle.SetIsOnWithoutNotify(false);
#else
            isCustomLevelsToggle.gameObject.SetActive(false);
            isCustomLevelsToggle.SetIsOnWithoutNotify(true);
#endif
            GetBrickButtons();
            SetUpBrickButtons();
            SetUpColorButtons();
            PopulateBackdropScenes();
            PopulateBackgroundMusic();
            PopulateBossSprites();
            PopulateBrickTypeDropDown();
            PopulateBonusTypeDropDown();
            PopulateLevels();
            SetDefaults();
        }

        /// <summary>
        ///  Sets default settings
        /// </summary>
        private void SetDefaults()
        {
            SetBrickColour(Color.red);
        }

        /// <summary>
        /// Updates the manager internal data with what's in the UI
        /// </summary>
        private void UpdateManager()
        {
            _levelEditorManager.MaxEnemies = Int32.Parse(maxEnemiesText.text);
            _levelEditorManager.MinEnemyTime = Int32.Parse(minTimeBetweenEnemiesText.text);
            _levelEditorManager.MaxEnemyTime = Int32.Parse(maxTimeBetweenEnemiesText.text);
            _levelEditorManager.IsBossLevel = levelIsBossLevelToggle.isOn;
            _levelEditorManager.LevelBossIndex = levelBossDropdown.value;
            _levelEditorManager.BackdropSceneIndex = backdropScenesDropDown.value;
            _levelEditorManager.BackgroundMusicIndex = backGroundMusicDropDown.value;
            _levelEditorManager.FileName = levelFileNameText.text;
            _levelEditorManager.LevelName = levelNameText.text;
            _levelEditorManager.IsCustomLevel = isCustomLevelsToggle.isOn;

            foreach (BrickButton brickButton in _allBrickButtons)
            {
                _levelEditorManager.BrickDataArray[brickButton.rowNumber, brickButton.columnNumber] =
                    brickButton.BrickData;
            }
        }

        /// <summary>
        /// Updates the UI with what's in the manager
        /// </summary>
        private void UpdateUserInterface()
        {
            maxEnemiesText.text = _levelEditorManager.MaxEnemies.ToString();
            minTimeBetweenEnemiesText.text = _levelEditorManager.MinEnemyTime.ToString();
            maxTimeBetweenEnemiesText.text = _levelEditorManager.MaxEnemyTime.ToString();
            levelIsBossLevelToggle.isOn = _levelEditorManager.IsBossLevel;
            levelBossDropdown.value = _levelEditorManager.BackdropSceneIndex;
            backdropScenesDropDown.value = _levelEditorManager.BackdropSceneIndex;
            backGroundMusicDropDown.value = _levelEditorManager.BackgroundMusicIndex;
            levelFileNameText.text = _levelEditorManager.FileName;
            levelNameText.text = _levelEditorManager.LevelName;

            foreach (BrickButton brickButton in _allBrickButtons)
            {
                brickButton.UpdateBrick(
                    _levelEditorManager.BrickDataArray[brickButton.rowNumber, brickButton.columnNumber]);
            }
        }

        /// <summary>
        /// Handler for Brick Color
        /// </summary>
        public void SetBrickColour(Color value)
        {
            mainColorButton.GetComponent<Image>().color = value;
        }

        /// <summary>
        /// Handler for Level Save
        /// </summary>
        public void LevelSaveClicked()
        {
            string fileName = levelFileNameText.text;
            if (string.IsNullOrEmpty(fileName))
            {
                alertText.DisplayAlert("Please enter a filename before saving!", true);
                return;
            }

            if (string.IsNullOrEmpty(levelNameText.text))
            {
                alertText.DisplayAlert("Please enter a level description before saving!", true);
                return;
            }

            if (!IsFilenameValid(fileName))
            {
                alertText.DisplayAlert("Level name contains invalid characters! Cannot save!", true);
                return;
            }

            UpdateManager();
            _levelEditorManager.SaveLevel();
            PopulateLevels();

            alertText.DisplayAlert($"Saved {fileName} successfully.", false);
        }

        /// <summary>
        /// Reload the level list if custom level is toggled
        /// </summary>
        public void CustomLevelToggled(bool newValue)
        {
            PopulateLevels();
        }

        /// <summary>
        /// Handler for Load Level
        /// </summary>
        public void LoadLevelClicked()
        {
            if (loadLevelDropDown.value < 0)
            {
                alertText.DisplayAlert("Please select a level to load!", true);
                return;
            }

            string levelName = loadLevelDropDown.options[loadLevelDropDown.value].text;
            bool isCustomLevel = isCustomLevelsToggle.isOn;

            levelFileNameText.text = levelName;

            // Load the level
            _levelEditorManager.LoadLevelByName(levelName, isCustomLevel);

            // Update the UI
            UpdateUserInterface();

            alertText.DisplayAlert($"Loaded {loadLevelDropDown.options[loadLevelDropDown.value].text} successfully.",
                false);
        }

        /// <summary>
        /// Handler for Delete Level button
        /// </summary>
        public void DeleteClicked()
        {
            deleteWindow.Show();
        }


        /// <summary>
        /// Handler to Share level button
        /// </summary>
        public void SharedClicked()
        {
            levelShareClickedEvent.Invoke();
        }

        public void AddClicked()
        {
            levelAddClickedEvent.Invoke();
        }

        /// <summary>
        /// Handler for Clear level
        /// </summary>
        public void ClearClicked()
        {
            maxEnemiesText.text = "0";
            minTimeBetweenEnemiesText.text = "0.0";
            maxTimeBetweenEnemiesText.text = "0.0";
            levelIsBossLevelToggle.isOn = false;
            levelBossDropdown.value = 0;
            backdropScenesDropDown.value = 0;
            backGroundMusicDropDown.value = 0;
            levelFileNameText.text = "";
            levelNameText.text = "";

            foreach (BrickButton brickButton in _allBrickButtons)
            {
                brickButton.Clear();
            }
        }

        /// <summary>
        /// Handler for Main Menu
        /// </summary>
        public void MainMenuClicked()
        {
            ReturnToMainMenu();
        }

        /// <summary>
        /// Calls for a file to be deleted. Call this from the Delete window
        /// </summary>
        public void DeleteFile()
        {
            string fileToDelete = loadLevelDropDown.options[loadLevelDropDown.value].text;
            bool isCustomLevel = isCustomLevelsToggle.isOn;

            _levelEditorManager.DeleteLevelFile(fileToDelete, isCustomLevel);
            alertText.DisplayAlert($"Deleted {fileToDelete} successfully.", false);
        }

        /// <summary>
        /// Return to Main Menu scene
        /// </summary>
        private void ReturnToMainMenu()
        {
            SceneManager.LoadScene(mainMenuScene);
        }

        /// <summary>
        /// Handle clicks to the Brick Grid
        /// </summary>
        private void BrickButtonHandler(int column, int row)
        {
            BrickData newBrickData = new BrickData
            {
                columnNumber = column,
                rowNumber = row,
                // Work out what data to send
                brickBonus = stampBonusToggle.isOn ? (BonusType)brickBonusDropDown.value : BonusType.None,
                HasBrickBonusChanged = stampBonusToggle.isOn,
                brickType = stampTypeToggle.isOn ? (BrickType)brickTypeDropDown.value : BrickType.Normal,
                HasBrickTypeChanged = stampTypeToggle.isOn,
                isEmptySlot = stampIsEmptyToggle.isOn && brickIsEmptyToggle.isOn,
                HasIsEmptySlotChanged = stampIsEmptyToggle.isOn,
                brickColor = stampColorToggle.isOn ? mainColorButton.GetComponent<Image>().color : Color.clear,
                HasBrickColorChanged = stampColorToggle.isOn
            };

            // Send to consumers
            RefreshBrick(newBrickData);
            // brickGridClickedEvent.Invoke(newBrickData);
        }

        /// <summary>
        /// Sets the Level Description
        /// </summary>
        public void SetLevelDesc(string levelDesc)
        {
            levelNameText.SetTextWithoutNotify(levelDesc);
        }

        /// <summary>
        /// Updates the selected music index
        /// </summary>
        public void SetBackgroundMusic(int musicIndex)
        {
            backGroundMusicDropDown.SetValueWithoutNotify(musicIndex);
        }

        /// <summary>
        /// Update the selected backdrop scene index
        /// </summary>
        public void SetBackdropScene(int sceneIndex)
        {
            backdropScenesDropDown.SetValueWithoutNotify(sceneIndex);
        }

        /// <summary>
        /// Updates the selected Boss Sprite index
        /// </summary>
        public void SetLevelBossSprite(int levelBossIndex)
        {
            levelBossDropdown.SetValueWithoutNotify(levelBossIndex);
        }

        /// <summary>
        /// Updates the selected Is Level Boss toggle
        /// </summary>
        public void SetIsBossLevel(bool value)
        {
            levelIsBossLevelToggle.SetIsOnWithoutNotify(value);
            levelBossDropdown.interactable = value;
        }

        /// <summary>
        /// Sets the Max Enemies
        /// </summary>
        public void SetMaxEnemies(int maxEnemies)
        {
            maxEnemiesText.SetTextWithoutNotify(maxEnemies.ToString());
        }

        /// <summary>
        /// Set the Min Enemy Time
        /// </summary>
        public void SetMinEnemyTime(float minEnemyTime)
        {
            minTimeBetweenEnemiesText.SetTextWithoutNotify(minEnemyTime.ToString());
        }

        /// <summary>
        /// Set the Max Enemy Time
        /// </summary>
        public void SetMaxEnemyTime(float maxEnemyTime)
        {
            maxTimeBetweenEnemiesText.SetTextWithoutNotify(maxEnemyTime.ToString());
        }

        /// <summary>
        /// Populate the drop down with current level list
        /// </summary>
        public void PopulateLevels()
        {
            // Get currently selected level
            int currentLevel = loadLevelDropDown.value;

            List<string> levelNames = _levelEditorManager.GetCurrentLevels(isCustomLevelsToggle.isOn);

            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            foreach (string fileName in levelNames)
            {
                options.Add(new TMP_Dropdown.OptionData(Path.GetFileNameWithoutExtension(fileName)));
            }

            loadLevelDropDown.options = options;

            // Restore the level
            if (currentLevel < options.Count)
            {
                loadLevelDropDown.SetValueWithoutNotify(currentLevel);
            }
            else
            {
                loadLevelDropDown.SetValueWithoutNotify(0);
            }
        }

        /// <summary>
        /// Refresh a single brick
        /// </summary>
        public void RefreshBrick(BrickData brickData)
        {
            brickUpdatedEvent.Invoke(brickData);
        }

        /// <summary>
        /// Refresh the Level Editor UI
        /// </summary>
        public void RefreshAllBricks(BrickData[,] brickDataArray)
        {
            BrickButton[] allButtons = grid.GetComponentsInChildren<BrickButton>(false);
            foreach (BrickButton button in allButtons)
            {
                button.BrickData = brickDataArray[button.columnNumber, button.rowNumber];
            }
        }


        /// <summary>
        /// Validate filename
        /// </summary>
        private bool IsFilenameValid(string filename)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                if (filename.Contains(c))
                {
                    Debug.Log($"File contains problem character: {c},\t{(int)c}");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Refresh list of Brick Buttons
        /// </summary>
        private void GetBrickButtons()
        {
            _allBrickButtons = grid.GetComponentsInChildren<BrickButton>(false);
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
            foreach (BonusData.BonusDef bonusDef in bonusData.bonuses)
            {
                options.Add(new TMP_Dropdown.OptionData(bonusDef.type.ToString(), bonusDef.levelEditorSprite,
                    Color.clear));
            }

            brickBonusDropDown.options = options;
        }

        /// <summary>
        /// Populates the available background sprites
        /// </summary>
        private void PopulateBackdropScenes()
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            foreach (string sceneName in backdropScenes.GetSceneNames())
            {
                options.Add(new TMP_Dropdown.OptionData(sceneName));
            }

            backdropScenesDropDown.options = options;
        }

        /// <summary>
        /// Populates the available boss sprites
        /// </summary>
        private void PopulateBossSprites()
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            foreach (EnemyData bossData in levelBossData.EnemyList)
            {
                options.Add(new TMP_Dropdown.OptionData(bossData.enemyName, bossData.sprite, Color.clear));
            }

            levelBossDropdown.options = options;
        }

        /// <summary>
        /// Populates the available background music
        /// </summary>
        private void PopulateBackgroundMusic()
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            foreach (PlayList.PlayListSong songp in backgroundMusicData.Songs)
            {
                options.Add(new TMP_Dropdown.OptionData(songp.SongName));
            }

            backGroundMusicDropDown.options = options;
        }

        /// <summary>
        /// Adds listeners to all Color Buttons
        /// </summary>
        private void SetUpColorButtons()
        {
            ColourButton[] allColourButtons = GetComponentsInChildren<ColourButton>();
            foreach (ColourButton button in allColourButtons)
            {
                button.ColorButtonClickedEvent.AddListener(SetBrickColour);
            }
        }

        /// <summary>
        /// Add listeners to all the Brick Buttons
        /// </summary>
        private void SetUpBrickButtons()
        {
            foreach (BrickButton button in _allBrickButtons)
            {
                button.brickButtonClickedEvent.AddListener(BrickButtonHandler);
                brickUpdatedEvent.AddListener(button.UpdateBrick);
                button.Clear();
            }
        }
    }
}
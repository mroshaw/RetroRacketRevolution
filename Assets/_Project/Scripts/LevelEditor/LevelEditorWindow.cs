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
        [BoxGroup("UI - Level")] [SerializeField] private TMP_InputField levelDescriptionText;
        [BoxGroup("UI - Level")] [SerializeField] private TMP_Dropdown loadLevelDropDown;
        [BoxGroup("UI - Level")] [SerializeField] private TMP_Dropdown backGroundSpriteDropDown;
        [BoxGroup("UI - Level")] [SerializeField] private TMP_Dropdown backGroundMusicDropDown;
        [BoxGroup("UI - Level")] [SerializeField] private TMP_InputField maxEnemiesText;
        [BoxGroup("UI - Level")] [SerializeField] private TMP_InputField minTimeBetweenEnemiesText;
        [BoxGroup("UI - Level")] [SerializeField] private TMP_InputField maxTimeBetweenEnemiesText;
        [BoxGroup("UI - Level")] [SerializeField] private Toggle isCustomLevelsToggle;
        [BoxGroup("UI - Level")] [SerializeField] private Toggle levelIsBossLevelToggle;
        [BoxGroup("UI - Level")] [SerializeField] private TMP_Dropdown levelBossSpriteDropDown;
        [BoxGroup("UI - Other")] [SerializeField] private ConfirmWindow deleteWindow;
        [BoxGroup("UI - Other")] [SerializeField] private AlertText alertText;
        [BoxGroup("UI - Other")] [SerializeField] private string mainMenuScene;

        [BoxGroup("Data")] [SerializeField] private BrickTypeData brickTypeData;
        [BoxGroup("Data")] [SerializeField] private BonusData bonusData;
        [BoxGroup("Data")] [SerializeField] private LevelBackgroundSprites backgroundData;
        [BoxGroup("Data")] [SerializeField] private PlayList backgroundMusicData;
        [BoxGroup("Data")] [SerializeField] private EnemiesData levelBossData;
        [FoldoutGroup("Events - Brick Grid")] public UnityEvent<BrickData> BrickGridClickedEvent;
        [FoldoutGroup("Events - Brick Grid")] public UnityEvent<BrickData> BrickUpdatedEvent;
        [FoldoutGroup("Events - Stamp")] public UnityEvent<bool> StampRewardChangedEvent;
        [FoldoutGroup("Events - Stamp")] public UnityEvent<bool> StampTypeChangedEvent;
        [FoldoutGroup("Events - Stamp")] public UnityEvent<bool> StampColorChangedEvent;
        [FoldoutGroup("Events - Stamp")] public UnityEvent<bool> StampIsEmptyChangedEvent;
        [FoldoutGroup("Events - Brick")] public UnityEvent<BrickType> BrickTypeChangedEvent;
        [FoldoutGroup("Events - Brick")] public UnityEvent<BonusType> BrickBonusChangedEvent;
        [FoldoutGroup("Events - Brick")] public UnityEvent<bool> BrickIsEmptyChangedEvent;
        [FoldoutGroup("Events - Brick")] public UnityEvent<Color> BrickColorChangedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent<int> LevelNumEnemiesChangedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent<float> LevelEnemyMinChangedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent<float> LevelEnemyMaxChangedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent<int> LevelBackgroundChangedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent<int> LevelBackgroundMusicChangedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent<int> LevelMusicPlayClickedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent<string> LevelFileNameChangedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent<string> LevelDescChangedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent<string, bool> LevelSaveClickedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent<string> LevelLoadFileNameChangedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent<bool> LevelCustomLevelChangedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent<string, bool> LevelLoadClickedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent<string, bool> LevelDeleteClickedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent LevelClearClickedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent<int> LevelBossChangedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent<bool> LevelIsBossLevelChangedEvent;
        [FoldoutGroup("Events - Other")] public UnityEvent MainMenuClickedEvent;
        [FoldoutGroup("Events - Other")] public UnityEvent LevelShareClickedEvent;
        [FoldoutGroup("Events - Other")] public UnityEvent LevelAddClickedEvent;

        private BrickButton[] _allBrickButtons;

        /// <summary>
        /// Initialise this component
        /// </summary>   
        private void Awake()
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
            PopulateBackgroundSprites();
            PopulateBackgroundMusic();
            PopulateBossSprites();
            PopulateBrickTypeDropDown();
            PopulateBonusTypeDropDown();
        }

        /// <summary>
        /// Configure the component on Start
        /// </summary>
        private void Start()
        {
        }

        /// <summary>
        /// Clean up on Destroy
        /// </summary>
        private void OnDestroy()
        {
        }

        /// <summary>
        /// Handler for Reward Stamp
        /// </summary>
        public void StampSetRewardHandler(bool value)
        {
            StampRewardChangedEvent.Invoke(value);
        }

        /// <summary>
        /// Handler for Type stamp
        /// </summary>
        public void StampSetTypeHandler(bool value)
        {
            StampTypeChangedEvent.Invoke(value);
        }

        /// <summary>
        /// Handler for Color stamp
        /// </summary>
        public void StampSetColorHandler(bool value)
        {
            StampColorChangedEvent.Invoke(value);
        }

        /// <summary>
        /// Handler for Empty stamp
        /// </summary>
        public void StampSetIsEmptyHandler(bool value)
        {
            StampIsEmptyChangedEvent.Invoke(value);
        }

        /// <summary>
        /// Handler for Brick Type
        /// </summary>
        public void BrickSetTypeHandler(int value)
        {
            BrickTypeChangedEvent.Invoke((BrickType)value);
        }

        /// <summary>
        /// Handler for Bonus Type
        /// </summary>
        public void BrickBonusTypeHandler(int value)
        {
            BrickBonusChangedEvent.Invoke((BonusType)value);
        }

        /// <summary>
        /// Handler for IsEmpty
        /// </summary>
        public void BrickIsEmptyTypeHandler(bool value)
        {
            BrickIsEmptyChangedEvent.Invoke(value);
        }

        /// <summary>
        /// Handler for Brick Color
        /// </summary>
        public void BrickColorHandler(Color value)
        {
            mainColorButton.GetComponent<Image>().color = value;
            BrickColorChangedEvent.Invoke(value);
        }

        /// <summary>
        /// Handler for Max Enemy
        /// </summary>
        public void LevelEnemiesHandler(string value)
        {
            LevelNumEnemiesChangedEvent.Invoke(int.Parse(value));
        }

        /// <summary>
        /// Handler for Min Enemy time
        /// </summary>
        public void LevelMinEnemyHandler(string value)
        {
            LevelEnemyMinChangedEvent.Invoke(float.Parse(value));
        }

        /// <summary>
        /// Handler for Max Enemy time
        /// </summary>
        public void LevelMaxEnemyHandler(string value)
        {
            LevelEnemyMaxChangedEvent.Invoke(float.Parse(value));
        }

        /// <summary>
        /// Handler for Level Background
        /// </summary>
        public void LevelBackgroundHandler(int value)
        {
            gridBackgroundImage.sprite = backgroundData.BackgroundSprites[value];
            LevelBackgroundChangedEvent.Invoke(value);
        }

        /// <summary>
        /// Handler for Level Background Music
        /// </summary>
        public void LevelBackgroundMusicHandler(int value)
        {
            LevelBackgroundMusicChangedEvent.Invoke(value);
        }

        /// <summary>
        /// Handler for Boss sprite
        /// </summary>
        public void LevelBossSpriteHandler(int value)
        {
            LevelBossChangedEvent.Invoke(value);
        }

        /// <summary>
        /// Handler for Is Boss Level
        /// </summary>
        public void LevelIsBossLevelHandler(bool value)
        {
            LevelIsBossLevelChangedEvent.Invoke(value);
        }

        /// <summary>
        /// Handler for Level Background Music button click
        /// </summary>
        public void LevelMusicPlayClickedHandler()
        {
            LevelMusicPlayClickedEvent.Invoke(backGroundMusicDropDown.value);
        }

        /// <summary>
        /// Handler for Level Name
        /// </summary>
        public void LevelNameHandler(string value)
        {
            LevelFileNameChangedEvent.Invoke(value);
        }

        /// <summary>
        /// Handler for Level Desc
        /// </summary>
        public void LevelDescHandler(string value)
        {
            LevelDescChangedEvent.Invoke(value);
        }

        /// <summary>
        /// Handler for Level Save
        /// </summary>
        public void LevelSaveHandler()
        {
            string fileName = levelFileNameText.text;
            if (string.IsNullOrEmpty(fileName))
            {
                alertText.DisplayAlert("Please enter a filename before saving!", true);
                return;
            }

            if (string.IsNullOrEmpty(levelDescriptionText.text))
            {
                alertText.DisplayAlert("Please enter a level description before saving!", true);
                return;
            }

            if (!IsFilenameValid(fileName))
            {
                alertText.DisplayAlert("Level name contains invalid characters! Cannot save!", true);
                return;
            }

            LevelSaveClickedEvent.Invoke(fileName, isCustomLevelsToggle.isOn);
            alertText.DisplayAlert($"Saved {fileName} successfully.", false);
        }

        /// <summary>
        /// Handler for Level Load Level
        /// </summary>
        public void LevelLoadLevelHandler(int value)
        {
            // levelFileNameText.text = loadLevelDropDown.options[value].text;
            LevelLoadFileNameChangedEvent.Invoke(loadLevelDropDown.options[value].text);
        }

        /// <summary>
        /// Handler for Custom Level
        /// </summary>
        public void LevelCustomLevelHandler(bool value)
        {
            LevelCustomLevelChangedEvent.Invoke(value);
        }

        /// <summary>
        /// Handler for Load Level
        /// </summary>
        public void LevelLoadHandler()
        {
            if (loadLevelDropDown.value < 0)
            {
                alertText.DisplayAlert("Please select a level to load!", true);
                return;
            }

            LevelLoadClickedEvent.Invoke(loadLevelDropDown.options[loadLevelDropDown.value].text,
                isCustomLevelsToggle.isOn);
            levelFileNameText.text = loadLevelDropDown.options[loadLevelDropDown.value].text;
            alertText.DisplayAlert($"Loaded {loadLevelDropDown.options[loadLevelDropDown.value].text} successfully.",
                false);
        }

        /// <summary>
        /// Handler for Delete Level button
        /// </summary>
        public void DeleteHandler()
        {
            deleteWindow.Show();
        }

        /// <summary>
        /// Handler to Share level button
        /// </summary>
        public void ShareHandler()
        {
            LevelShareClickedEvent.Invoke();
        }

        public void AddHandler()
        {
            LevelAddClickedEvent.Invoke();
        }

        /// <summary>
        /// Handler for Clear level
        /// </summary>
        public void LevelClearHandler()
        {
            levelFileNameText.text = "";
            LevelClearClickedEvent.Invoke();
        }

        /// <summary>
        /// Handler for Main Menu
        /// </summary>
        public void MainMenuHandler()
        {
            MainMenuClickedEvent.Invoke();
        }

        /// <summary>
        /// Calls for a file to be deleted
        /// </summary>
        public void DeleteFile()
        {
            string fileToDelete = loadLevelDropDown.options[loadLevelDropDown.value].text;
            LevelDeleteClickedEvent.Invoke(fileToDelete, isCustomLevelsToggle.isOn);
            alertText.DisplayAlert($"Deleted {fileToDelete} successfully.", false);
        }

        /// <summary>
        /// Return to Main Menu scene
        /// </summary>
        public void MainMenu()
        {
            SceneManager.LoadScene(mainMenuScene);
        }

        /// <summary>
        /// Handle clicks to the Brick Grid
        /// </summary>
        public void BrickButtonHandler(int column, int row)
        {
            BrickData newBrickData = new BrickData();
            newBrickData.ColumnNumber = column;
            newBrickData.RowNumber = row;

            // Work out what data to send
            newBrickData.BrickBonus = stampBonusToggle.isOn ? (BonusType)brickBonusDropDown.value : BonusType.None;
            newBrickData.HasBrickBonusChanged = stampBonusToggle.isOn;
            newBrickData.BrickType = stampTypeToggle.isOn ? (BrickType)brickTypeDropDown.value : BrickType.Normal;
            newBrickData.HasBrickTypeChanged = stampTypeToggle.isOn;
            newBrickData.IsEmptySlot = stampIsEmptyToggle.isOn ? brickIsEmptyToggle.isOn : false;
            newBrickData.HasIsEmptySlotChanged = stampIsEmptyToggle.isOn;
            newBrickData.BrickColor = stampColorToggle.isOn ? mainColorButton.GetComponent<Image>().color : Color.clear;
            newBrickData.HasBrickColorChanged = stampColorToggle.isOn;
            // Send to consumers
            BrickGridClickedEvent.Invoke(newBrickData);
        }

        /// <summary>
        /// Sets the Level Description
        /// </summary>
        public void SetLevelDesc(string levelDesc)
        {
            levelDescriptionText.SetTextWithoutNotify(levelDesc);
        }

        /// <summary>
        /// Updates the grid background to match the background sprite choice
        /// </summary>
        public void SetBackground(int spriteIndex)
        {
            backGroundSpriteDropDown.SetValueWithoutNotify(spriteIndex);
            gridBackgroundImage.sprite = backgroundData.BackgroundSprites[spriteIndex];
        }

        /// <summary>
        /// Updates the selected music index
        /// </summary>
        public void SetBackgroundMusic(int musicIndex)
        {
            backGroundMusicDropDown.SetValueWithoutNotify(musicIndex);
        }

        /// <summary>
        /// Updates the selected Boss Sprite index
        /// </summary>
        public void SetLevelBossSprite(int levelBossIndex)
        {
            levelBossSpriteDropDown.SetValueWithoutNotify(levelBossIndex);
        }

        /// <summary>
        /// Updates the selected Is Level Boss toggle
        /// </summary>
        /// <param name="value"></param>
        public void SetIsBossLevel(bool value)
        {
            levelIsBossLevelToggle.SetIsOnWithoutNotify(value);
            levelBossSpriteDropDown.interactable = value;
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
        public void SetCurrentLevels(List<string> levelNames)
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            foreach (string fileName in levelNames)
            {
                options.Add(new TMP_Dropdown.OptionData(Path.GetFileNameWithoutExtension(fileName)));
            }

            loadLevelDropDown.options = options;
            loadLevelDropDown.SetValueWithoutNotify(0);
            // levelFileNameText.SetTextWithoutNotify(options[0].text);
        }

        /// <summary>
        /// Refresh a single brick
        /// </summary>
        public void RefreshBrick(BrickData brickData)
        {
            BrickUpdatedEvent.Invoke(brickData);
        }

        /// <summary>
        /// Refresh the Level Editor UI
        /// </summary>
        public void RefreshAllBricks(BrickData[,] brickDataArray)
        {
            BrickButton[] allButtons = grid.GetComponentsInChildren<BrickButton>(false);
            foreach (BrickButton button in allButtons)
            {
                button.BrickData = brickDataArray[button.ColumnNumber, button.RowNumber];
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
                options.Add(new TMP_Dropdown.OptionData(bonusDef.type.ToString(), bonusDef.spawnSprite, Color.clear));
            }

            brickBonusDropDown.options = options;
        }

        /// <summary>
        /// Populates the available background sprites
        /// </summary>
        private void PopulateBackgroundSprites()
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            foreach (Sprite sprite in backgroundData.BackgroundSprites)
            {
                options.Add(new TMP_Dropdown.OptionData(sprite.name, sprite, Color.clear));
            }

            backGroundSpriteDropDown.options = options;
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

            levelBossSpriteDropDown.options = options;
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
                button.ColorButtonClickedEvent.AddListener(BrickColorHandler);
            }
        }

        /// <summary>
        /// Add listeners to all the Brick Buttons
        /// </summary>
        private void SetUpBrickButtons()
        {
            foreach (BrickButton button in _allBrickButtons)
            {
                button.BrickButtonClickedEvent.AddListener(BrickButtonHandler);
                BrickUpdatedEvent.AddListener(button.UpdateBrick);
            }
        }
    }
}
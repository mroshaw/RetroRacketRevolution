using DaftApplesGames.RetroRacketRevolution.Bonuses;
using DaftApplesGames.RetroRacketRevolution.Bricks;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;
using DaftApplesGames.RetroRacketRevolution.Levels;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine.SceneManagement;

namespace DaftApplesGames.RetroRacketRevolution.LevelEditor
{
    public class LevelEditorWindow : MonoBehaviour
    {
        // Public serializable properties
        #region UI
        [BoxGroup("UI - Brick Grid")] public GameObject grid;
        [BoxGroup("UI - Brick Grid")] public Image gridBackgroundImage;

        [BoxGroup("UI - Stamp")] public Toggle stampBonusToggle;
        [BoxGroup("UI - Stamp")] public Toggle stampTypeToggle;
        [BoxGroup("UI - Stamp")] public Toggle stampColorToggle;
        [BoxGroup("UI - Stamp")] public Toggle stampIsEmptyToggle;

        [BoxGroup("UI - Brick")] public TMP_Dropdown brickTypeDropDown;
        [BoxGroup("UI - Brick")] public TMP_Dropdown brickBonusDropDown;
        [BoxGroup("UI - Brick")] public Toggle brickIsEmptyToggle;
        [BoxGroup("UI - Brick")] public Button mainColorButton;

        [BoxGroup("UI - Level")] public TMP_InputField levelFileNameText;
        [BoxGroup("UI - Level")] public TMP_InputField levelDescriptionText;
        [BoxGroup("UI - Level")] public TMP_Dropdown loadLevelDropDown;
        [BoxGroup("UI - Level")] public TMP_Dropdown backGroundSpriteDropDown;
        [BoxGroup("UI - Level")] public TMP_InputField maxEnemiesText;
        [BoxGroup("UI - Level")] public TMP_InputField minTimeBetweenEnemiesText;
        [BoxGroup("UI - Level")] public TMP_InputField maxTimeBetweenEnemiesText;
        [BoxGroup("UI - Level")] public Toggle isCustomLevels;

        [BoxGroup("UI - Other")] public DeleteWindow deleteWindow;
        [BoxGroup("UI - Other")] public AlertText alertText;
        #endregion
        #region Data
        [BoxGroup("Data")] public BrickTypeData brickTypeData;
        [BoxGroup("Data")] public BonusData bonusData;
        [BoxGroup("Data")] public LevelBackgroundSprites backgroundData;
        #endregion
        #region Events
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
        [FoldoutGroup("Events - Level")] public UnityEvent<string> LevelFileNameChangedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent<string> LevelDescChangedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent<string, bool> LevelSaveClickedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent<string> LevelLoadFileNameChangedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent<bool> LevelCustomLevelChangedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent<string, bool> LevelLoadClickedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent<string, bool> LevelDeleteClickedEvent;
        [FoldoutGroup("Events - Level")] public UnityEvent LevelClearClickedEvent;
        [FoldoutGroup("Events - Other")] public UnityEvent MainMenuClickedEvent;
        #endregion
        
        // Public properties

        // Private fields
        private BrickButton[] _allBrickButtons;

        #region UnityMethods
        /// <summary>
        /// Initialise this component
        /// </summary>   
        private void Awake()
        {
            // Enable switching between custom and OG levels in Editor only
#if UNITY_EDITOR
            isCustomLevels.gameObject.SetActive(true);
            isCustomLevels.SetIsOnWithoutNotify(false);
#else
            isCustomLevels.gameObject.SetActive(false);
            isCustomLevels.SetIsOnWithoutNotify(true);
#endif
            GetBrickButtons();
            SetUpBrickButtons();
            SetUpColorButtons();
            PopulateBackgroundSprites();
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
        #endregion
        #region PublicMethods
        #region UiHandlers
        /// <summary>
        /// Handler for Reward Stamp
        /// </summary>
        /// <param name="value"></param>
        public void StampSetRewardHandler(bool value)
        {
            StampRewardChangedEvent.Invoke(value);
        }

        /// <summary>
        /// Handler for Type stamp
        /// </summary>
        /// <param name="value"></param>
        public void StampSetTypeHandler(bool value)
        {
            StampTypeChangedEvent.Invoke(value);
        }

        /// <summary>
        /// Handler for Color stamp
        /// </summary>
        /// <param name="value"></param>
        public void StampSetColorHandler(bool value)
        {
            StampColorChangedEvent.Invoke(value);
        }

        /// <summary>
        /// Handler for Empty stamp
        /// </summary>
        /// <param name="value"></param>
        public void StampSetIsEmptyHandler(bool value)
        {
            StampIsEmptyChangedEvent.Invoke(value);
        }

        /// <summary>
        /// Handler for Brick Type
        /// </summary>
        /// <param name="value"></param>
        public void BrickSetTypeHandler(int value)
        {
            BrickTypeChangedEvent.Invoke((BrickType)value);
        }

        /// <summary>
        /// Handler for Bonus Type
        /// </summary>
        /// <param name="value"></param>
        public void BrickBonusTypeHandler(int value)
        {
            BrickBonusChangedEvent.Invoke((BonusType)value);
        }

        /// <summary>
        /// Handler for IsEmpty
        /// </summary>
        /// <param name="value"></param>
        public void BrickIsEmptyTypeHandler(bool value)
        {
            BrickIsEmptyChangedEvent.Invoke(value);
        }

        /// <summary>
        /// Handler for Brick Color
        /// </summary>
        /// <param name="value"></param>
        public void BrickColorHandler(Color value)
        {
            mainColorButton.GetComponent<Image>().color = value;
            BrickColorChangedEvent.Invoke(value);
        }

        /// <summary>
        /// Handler for Max Enemy
        /// </summary>
        /// <param name="value"></param>
        public void LevelEnemiesHandler(string value)
        {
            LevelNumEnemiesChangedEvent.Invoke(int.Parse(value));
        }

        /// <summary>
        /// Handler for Min Enemy time
        /// </summary>
        /// <param name="value"></param>
        public void LevelMinEnemyHandler(string value)
        {
            LevelEnemyMinChangedEvent.Invoke(float.Parse(value));
        }

        /// <summary>
        /// Handler for Max Enemy time
        /// </summary>
        /// <param name="value"></param>
        public void LevelMaxEnemyHandler(string value)
        {
            LevelEnemyMaxChangedEvent.Invoke(float.Parse(value));
        }

        /// <summary>
        /// Handler for Level Background
        /// </summary>
        /// <param name="value"></param>
        public void LevelBackgroundHandler(int value)
        {
            gridBackgroundImage.sprite = backgroundData.BackgroundSprites[value];
            LevelBackgroundChangedEvent.Invoke(value);
        }

        /// <summary>
        /// Handler for Level Name
        /// </summary>
        /// <param name="value"></param>
        public void LevelNameHandler(string value)
        {
            LevelFileNameChangedEvent.Invoke(value);
        }

        /// <summary>
        /// Handler for Level Desc
        /// </summary>
        /// <param name="value"></param>
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

            LevelSaveClickedEvent.Invoke(fileName, isCustomLevels.isOn);
            alertText.DisplayAlert($"Saved {fileName} successfully.", false);
        }

        /// <summary>
        /// Handler for Level Load Level
        /// </summary>
        /// <param name="value"></param>
        public void LevelLoadLevelHandler(int value)
        {
            // levelFileNameText.text = loadLevelDropDown.options[value].text;
            LevelLoadFileNameChangedEvent.Invoke(loadLevelDropDown.options[value].text);
        }

        /// <summary>
        /// Handler for Custom Level
        /// </summary>
        /// <param name="value"></param>
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
            LevelLoadClickedEvent.Invoke(loadLevelDropDown.options[loadLevelDropDown.value].text, isCustomLevels.isOn);
            levelFileNameText.text = loadLevelDropDown.options[loadLevelDropDown.value].text;
            alertText.DisplayAlert($"Loaded {loadLevelDropDown.options[loadLevelDropDown.value].text} successfully.", false);
        }

        /// <summary>
        /// Handler for Delete Level button
        /// </summary>
        public void DeleteHandler()
        {
            deleteWindow.Show();
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
        #endregion
        #region UiMethods
        /// <summary>
        /// Calls for a file to be deleted
        /// </summary>
        public void DeleteFile()
        {
            string fileToDelete = loadLevelDropDown.options[loadLevelDropDown.value].text;
            LevelDeleteClickedEvent.Invoke(fileToDelete, isCustomLevels.isOn);
            alertText.DisplayAlert($"Deleted {fileToDelete} successfully.", false);
        }

        /// <summary>
        /// Return to Main Menu scene
        /// </summary>
        public void MainMenu()
        {
            SceneManager.LoadScene("MainMenuScene");
        }

        /// <summary>
        /// Handle clicks to the Brick Grid
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
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
        #endregion
        #region UiUpdates
        /// <summary>
        /// Sets the Level Description
        /// </summary>
        /// <param name="levelDesc"></param>
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
        /// Sets the Max Enemies
        /// </summary>
        /// <param name="maxEnemies"></param>
        public void SetMaxEnemies(int maxEnemies)
        {
            maxEnemiesText.SetTextWithoutNotify(maxEnemies.ToString());
        }

        /// <summary>
        /// Set the Min Enemy Time
        /// </summary>
        /// <param name="minEnemyTime"></param>
        public void SetMinEnemyTime(float minEnemyTime)
        {
            minTimeBetweenEnemiesText.SetTextWithoutNotify(minEnemyTime.ToString());
        }

        /// <summary>
        /// Set the Max Enemy Time
        /// </summary>
        /// <param name="maxEnemyTime"></param>
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
         /// <param name="brickData"></param>
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
        #endregion
        #endregion
        #region PrivateMethods

        /// <summary>
        /// Validate filename
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
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
            foreach (BonusData.BonusDef bonusDef in bonusData.Bonuses)
            {
                options.Add(new TMP_Dropdown.OptionData(bonusDef.Type.ToString(), bonusDef.SpawnSprite));
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
                options.Add(new TMP_Dropdown.OptionData(sprite.name, sprite));
            }
            backGroundSpriteDropDown.options = options;
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
        #endregion
    }
}

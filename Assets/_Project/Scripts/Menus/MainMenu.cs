using System;
using System.Collections.Generic;
using DaftAppleGames.RetroRacketRevolution.Game;
using DaftAppleGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DaftAppleGames.RetroRacketRevolution.Menus
{
    public class MainMenu : WindowBase
    {
        [BoxGroup("UI Settings")] public TMP_Dropdown p1ControlsDropDown;
        [BoxGroup("UI Settings")] public TMP_Dropdown p2ControlsDropDown;
        [BoxGroup("UI Settings")] public TextMeshProUGUI versionText;
        [BoxGroup("Control Defaults")] public ControlScheme windowsControlSchemeDefaultP1;
        [BoxGroup("Control Defaults")] public ControlScheme windowsControlSchemeDefaultP2;
        [BoxGroup("Control Defaults")] public ControlScheme linuxControlSchemeDefaultP1;
        [BoxGroup("Control Defaults")] public ControlScheme linuxControlSchemeDefaultP2;
        [BoxGroup("Control Defaults")] public ControlScheme androidControlSchemeDefault;

        [BoxGroup("Game Data")] public GameData gameData;

        private static string P1ControlsIndexKey = "Player1Controls";
        private static string P2ControlsIndexKey = "Player2Controls";

        private int _p1ControlIndex;
        private int _p2ControlIndex;

        public override void Awake()
        {
            base.Awake();
            // Disable Touch for anything other than mobile
#if !PLATFORM_ANDROID
            RemoveDropDownOption(p1ControlsDropDown, 3);
            RemoveDropDownOption(p2ControlsDropDown, 3);
#endif
        }

        /// <summary>
        /// Setup the Main Menu
        /// </summary>
        public override void Start()
        {
            base.Start();
            // Update the version text
            versionText.text = $"Build: {Version.Parse(Application.version).ToString()}";
 
            // Set the default controls, depending on platform
            SetControlSchemes();

            // Show the main menu
            Show();
        }

        /// <summary>
        /// Sets up the control scheme drop downs
        /// </summary>
        private void SetControlSchemes()
        {
            LoadControlSettings();
            UpdateP1Controls((ControlScheme)_p1ControlIndex);
            UpdateP2Controls((ControlScheme)_p2ControlIndex);
        }

        /// <summary>
        /// Keep the mouse cursor visible
        /// </summary>
        public void Update()
        {
            Cursor.visible = true;
        }

        /// <summary>
        /// Set the UI state
        /// </summary>
        /// <param name="controlScheme"></param>
        private void UpdateP1Controls(ControlScheme controlScheme)
        {
            p1ControlsDropDown.SetValueWithoutNotify((int)controlScheme);
            gameData.playerOneControlScheme = controlScheme.ToString();
        }

        /// <summary>
        /// Set the UI state
        /// </summary>
        /// <param name="controlScheme"></param>
        private void UpdateP2Controls(ControlScheme controlScheme)
        {
            p2ControlsDropDown.SetValueWithoutNotify((int)controlScheme);
            gameData.playerTwoControlScheme = controlScheme.ToString();
        }

        /// <summary>
        /// Public method for UI event
        /// </summary>
        /// <param name="controlSchemeIndex"></param>
        public void SetP1Controls(int controlSchemeIndex)
        {
            SetP1Controls((ControlScheme)controlSchemeIndex);
            SaveControlSettings();
        }

        /// <summary>
        /// Public method for UI event
        /// </summary>
        /// <param name="controlSchemeIndex"></param>
        public void SetP2Controls(int controlSchemeIndex)
        {
            SetP2Controls((ControlScheme)controlSchemeIndex);
            SaveControlSettings();
        }

        /// <summary>
        /// Sets the Player 1 Control Scheme
        /// </summary>
        /// <param name="controlScheme"></param>
        public void SetP1Controls(ControlScheme controlScheme)
        {
            gameData.playerOneControlScheme = controlScheme.ToString();

            if (p1ControlsDropDown.value == p2ControlsDropDown.value)
            {
                p2ControlsDropDown.value = p2ControlsDropDown.value < 2 ? p2ControlsDropDown.value + 1 : 0;
            }

            _p1ControlIndex = p1ControlsDropDown.value;
            _p2ControlIndex = p2ControlsDropDown.value;

            p1ControlsDropDown.RefreshShownValue();
            p2ControlsDropDown.RefreshShownValue();
        }

        /// <summary>
        /// Sets the Player 2 Control Scheme
        /// </summary>
        /// <param name="controlScheme"></param>
        public void SetP2Controls(ControlScheme controlScheme)
        {
            gameData.playerTwoControlScheme = controlScheme.ToString();

            if (p1ControlsDropDown.value == p2ControlsDropDown.value)
            {
                p1ControlsDropDown.value = p1ControlsDropDown.value < 2 ? p1ControlsDropDown.value + 1 : 0;
            }

            _p1ControlIndex = p1ControlsDropDown.value;
            _p2ControlIndex = p2ControlsDropDown.value;

            p1ControlsDropDown.RefreshShownValue();
            p2ControlsDropDown.RefreshShownValue();
        }

        /// <summary>
        /// Disables the dropdown item at given index
        /// </summary>
        /// <param name="dropDown"></param>
        /// <param name="optionIndex"></param>
        private void RemoveDropDownOption(TMP_Dropdown dropDown, int optionIndex)
        {
            List<TMP_Dropdown.OptionData> optionList = dropDown.options;
            optionList.RemoveAt(optionIndex);
            dropDown.options = optionList;
        }

        /// <summary>
        /// Start a one player game
        /// </summary>
        public void Start1P()
        {
            gameData.isTwoPlayer = false;
        }

        /// <summary>
        /// Start the game
        /// </summary>
        public void StartGame()
        {
            SceneManager.LoadScene("GameScene");
        }

        /// <summary>
        /// Start a two player game
        /// </summary>
        public void Start2P()
        {
            gameData.isTwoPlayer = true;
        }

        /// <summary>
        /// Open the Level Editor
        /// </summary>
        public void OpenLevelEditor()
        {
            SceneManager.LoadScene("LevelEditorScene");
        }

        /// <summary>
        /// Exit to desktop
        /// </summary>
        public void ExitToDesktop()
        {
            Application.Quit();
        }

        /// <summary>
        /// Save control settings
        /// </summary>
        private void SaveControlSettings()
        {
            PlayerPrefs.SetInt(P1ControlsIndexKey, _p1ControlIndex);
            PlayerPrefs.SetInt(P2ControlsIndexKey, _p2ControlIndex);
        }

        /// <summary>
        /// Load control settings
        /// </summary>
        private void LoadControlSettings()
        {
#if UNITY_STANDALONE_LINUX
            _p1ControlIndex = PlayerPrefs.GetInt(P1ControlsIndexKey, (int)linuxControlSchemeDefaultP1);
            _p2ControlIndex = PlayerPrefs.GetInt(P2ControlsIndexKey, (int)linuxControlSchemeDefaultP2);
#elif PLATFORM_ANDROID
            _p1ControlIndex = PlayerPrefs.GetInt(P1ControlsIndexKey, (int)androidControlSchemeDefault);
#else
            _p1ControlIndex = PlayerPrefs.GetInt(P1ControlsIndexKey, (int)windowsControlSchemeDefaultP1);
            _p2ControlIndex = PlayerPrefs.GetInt(P2ControlsIndexKey, (int)windowsControlSchemeDefaultP2);
#endif
        }
    }
}

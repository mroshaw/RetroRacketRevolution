using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DaftApplesGames.RetroRacketRevolution.Menus
{
    public class MainMenu : WindowBase
    {
        [BoxGroup("UI Settings")] public TMP_Dropdown p1ControlsDropDown;
        [BoxGroup("UI Settings")] public TMP_Dropdown p2ControlsDropDown;

        /// <summary>
        /// Setup the Main Menu
        /// </summary>
        public void Start()
        {
            Show();
        }

        /// <summary>
        /// Keep the mouse cursor visible
        /// </summary>
        public void Update()
        {
            Cursor.visible = true;
        }

        /// <summary>
        /// Sets the Player 1 Control Scheme
        /// </summary>
        public void SetP1Controls(int controlIndex)
        {
            GameController.Instance.PlayerOneControlScheme = GetControlScheme(controlIndex);
            if (p1ControlsDropDown.value == p2ControlsDropDown.value)
            {
                p2ControlsDropDown.value = p2ControlsDropDown.value < 2 ? p2ControlsDropDown.value + 1 : 0;
            }
        }

        /// <summary>
        /// Sets the Player 2 Control Scheme
        /// </summary>
        /// <param name="controlIndex"></param>
        public void SetP2Controls(int controlIndex)
        {
            GameController.Instance.PlayerTwoControlScheme = GetControlScheme(controlIndex);
            if (p1ControlsDropDown.value == p2ControlsDropDown.value)
            {
                p1ControlsDropDown.value = p1ControlsDropDown.value < 2 ? p1ControlsDropDown.value + 1 : 0;
            }
        }

        /// <summary>
        /// Disables the dropdown item at given index
        /// </summary>
        /// <param name="dropDown"></param>
        /// <param name="controlIndex"></param>
        private void DisableDropDownToggle(TMP_Dropdown dropDown, int controlIndex)
        {
            Toggle[] toggles = dropDown.GetComponentsInChildren<Toggle>(true);
            foreach (Toggle toggle in toggles)
            {
                toggle.interactable = true;
            }
            toggles[controlIndex].interactable = false;
        }

        /// <summary>
        /// Gets the control scheme name
        /// </summary>
        /// <param name="controlIndex"></param>
        /// <returns></returns>
        private string GetControlScheme(int controlIndex)
        {
            switch (controlIndex)
            {
                case 0:
                    return "Keyboard";
                case 1:
                    return "Mouse";
                case 2:
                    return "Gamepad";
                default:
                    return "Keyboard";
            }
        }
        /// <summary>
        /// Start a one player game
        /// </summary>
        public void Start1P()
        {
            GameController.Instance.IsTwoPlayer = false;
            SceneManager.LoadScene("GameScene");
        }

        /// <summary>
        /// Start a two player game
        /// </summary>
        public void Start2P()
        {
            GameController.Instance.IsTwoPlayer = true;
            SceneManager.LoadScene("GameScene");
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
    }
}

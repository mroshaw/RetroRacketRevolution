using DaftAppleGames.RetroRacketRevolution.Game;
using DaftAppleGames.UserInterface.MainMenu;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DaftAppleGames.RetroRacketRevolution.Menus
{
    public class MainMenu : MainMenuController
    {
        [BoxGroup("Game Config")] [SerializeField] private GameConfig gameConfig;
        [BoxGroup("Game Data")] [SerializeField] private GameData gameData;
        /// <summary>
        /// Start a one player game
        /// </summary>
        public void Start1P()
        {
            gameData.isTwoPlayer = false;
            StartGame();
        }

        /// <summary>
        /// Start the game
        /// </summary>
        private void StartGame()
        {
            SceneManager.LoadScene(gameConfig.gameScene);
        }

        /// <summary>
        /// Start a two player game
        /// </summary>
        public void Start2P()
        {
            gameData.isTwoPlayer = true;
            StartGame();
        }

        /// <summary>
        /// Open the Level Editor
        /// </summary>
        public void OpenLevelEditor()
        {
            SceneManager.LoadScene(gameConfig.levelEditorScene);
        }
    }
}
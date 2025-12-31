using DaftAppleGames.RetroRacketRevolution.Game;
using DaftAppleGames.UserInterface;
using DaftAppleGames.UserInterface.PauseGame;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace DaftAppleGames.RetroRacketRevolution.Menus
{
    public class PauseGame : PauseGameController
    {
        [BoxGroup("Game Config")] [SerializeField] private GameConfig gameConfig;
        [BoxGroup("Settings")] [SerializeField] private CanvasFader canvasFader;

        /// <summary>
        /// Toggles the pause state
        /// </summary>
        public void TogglePause(InputAction.CallbackContext context)
        {
            TogglePauseGame();
        }

        protected override void ReturnToMainMenu()
        {
            canvasFader.FadeOut(null, LoadMainMenu);
        }

        private void LoadMainMenu()
        {
            UnPauseGame();
            SceneManager.LoadScene(gameConfig.menuScene);
        }
    }
}
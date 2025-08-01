using DaftAppleGames.RetroRacketRevolution.Game;
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
        
        /// <summary>
        /// Toggles the pause state
        /// </summary>
        public void TogglePause(InputAction.CallbackContext context)
        {
            TogglePauseGame();
        }

        protected override void ReturnToMainMenu()
        {
            SceneManager.LoadScene(gameConfig.menuScene);
        }
    }
}

using DaftAppleGames.Levels;
using DaftAppleGames.RetroRacketRevolution.Game;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DaftAppleGames.Game
{
    public class InitGame : MonoBehaviour
    {
        [BoxGroup("Settings")] [SerializeField] private GameConfig gameConfig;

        private void Start()
        {
            LevelDataResources levelDataResources = GetComponent<LevelDataResources>();
            levelDataResources.UnpackAllLevels();
        }

        public void InitDone()
        {
            SceneManager.LoadScene(gameConfig.menuScene);
        }
    }
}
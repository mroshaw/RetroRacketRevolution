using DaftAppleGames.RetroRacketRevolution.Levels;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DaftAppleGames.RetroRacketRevolution.Game
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
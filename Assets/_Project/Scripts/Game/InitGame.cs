using DaftAppleGames.Levels;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DaftAppleGames.Game
{
    public class InitGame : MonoBehaviour
    {
        [BoxGroup("Settings")] [SerializeField] private string mainMenuScene;
        private void Start()
        {
            LevelDataResources levelDataResources = GetComponent<LevelDataResources>();
            levelDataResources.UnpackAllLevels();
        }

        public void InitDone()
        {
            SceneManager.LoadScene(mainMenuScene);
        }
    }
}
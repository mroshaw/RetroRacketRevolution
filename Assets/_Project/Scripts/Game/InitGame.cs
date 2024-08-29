using DaftAppleGames.Levels;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace DaftAppleGames.Game
{
    public class InitGame : MonoBehaviour
    {
        #region Unity events
        private void Start()
        {
            LevelDataResources levelDataResources = GetComponent<LevelDataResources>();
            levelDataResources.UnpackLevelData();

        }
        #endregion

    }
}
using UnityEngine;

namespace DaftApplesGames.RetroRacketRevolution.Utils
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        /// <summary>
        /// Set DontDestroyOnLoad for this GameObject
        /// </summary>
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}

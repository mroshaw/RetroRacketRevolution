using UnityEngine;
using Sirenix.OdinInspector;

namespace DaftAppleGames.RetroRacketRevolution.Menus
{
    public class UiRotate : MonoBehaviour
    {
        // Public serializable properties
        [BoxGroup("General Settings")]
        public float rotateSpeed;
        
        /// <summary>
        /// Configure the component on Start
        /// </summary>
        private void Update()
        {
            transform.Rotate(0, 0, rotateSpeed);
        }
    }
}

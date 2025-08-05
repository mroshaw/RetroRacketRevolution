using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

namespace DaftAppleGames.RetroRacketRevolution.Effects
{
    [ExecuteInEditMode] public class FollowMouse : MonoBehaviour
    {
        // Public serializable properties
        [BoxGroup("General Settings")] public float speed = 8.0f;
        [BoxGroup("General Settings")] public float distanceFromCamera = 5.0f;


        /// <summary>
        /// Follow the mouse around
        /// </summary>
        public void Update()
        {
            Vector3 mousePosition = Mouse.current.position.value;
            mousePosition.z = distanceFromCamera;
            Vector3 mouseScreenToWorld = Camera.main.ScreenToWorldPoint(mousePosition);

            Vector3 position = Vector3.Lerp(transform.position, mouseScreenToWorld,
                1.0f - Mathf.Exp(-speed * Time.deltaTime));

            transform.position = position;
        }
    }
}
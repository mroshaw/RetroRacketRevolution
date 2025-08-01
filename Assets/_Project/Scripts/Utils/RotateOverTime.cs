using Sirenix.OdinInspector;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.Utils
{
    /// <summary>
    /// Simple component to rotate an object transform over time
    /// </summary>
    public class RotateOverTime : MonoBehaviour
    {
        [BoxGroup("Settings")] [SerializeField] private bool rotateOnStart = true;
        [BoxGroup("Settings")] [SerializeField] private float rotateSpeed = 1.0f;
        
        private bool _rotating = false;
        
        private void Start()
        {
            _rotating = rotateOnStart;
        }

        private void Update()
        {
            if (!_rotating)
            {
                return;
            }
            transform.Rotate(Vector3.up * (rotateSpeed * Time.deltaTime), Space.World);
        }

        [Button("Start Rotating")]
        public void StartRotating()
        {
            _rotating = true;
        }
        
        [Button("Stop Rotating")]
        public void StopRotating()
        {
            _rotating = false;
        }
        
    }
}

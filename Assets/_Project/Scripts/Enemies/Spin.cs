using Sirenix.OdinInspector;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution
{
    public class Spin : MonoBehaviour
    {
        [BoxGroup("Settings")] [SerializeField] private float spinSpeed;
        [BoxGroup("Settings")] [SerializeField] private bool spinOnStart = true;

        private bool _isSpinning;

        private void Start()
        {
            _isSpinning = spinOnStart;
        }

        private void Update()
        {
            if (!_isSpinning)
            {
                return;
            }

            transform.Rotate(0.0f, spinSpeed * Time.deltaTime, 0.0f, Space.Self);
        }
    }
}
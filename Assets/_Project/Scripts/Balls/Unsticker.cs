using System;
using DaftApplesGames.RetroRacketRevolution.Balls;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DaftApplesGames.RetroRacketRevolution
{
    public class Unsticker : MonoBehaviour
    {

        [BoxGroup("Settings")] public float stuckTimeBeforeIntervention = 3.0f;
        [BoxGroup("Settings")] public float verticalVelocityThreshold = 0.01f;
        [BoxGroup("Settings")] public float horizontalVelocityThreshold = 0.01f;
        [BoxGroup("Settings")] public float nudgeForce = 1.0f;

        [BoxGroup("Debug")] [SerializeField] private float _rbVelocityX;
        [BoxGroup("Debug")] [SerializeField] private float _rbVelocityY;


        [BoxGroup("Debug")][SerializeField] private float _horizontalStuckTime;
        [BoxGroup("Debug")][SerializeField] private float _verticalStuckTime;

        private Ball _ball;

        private Rigidbody2D _rb;

        /// <summary>
        /// Initialise this component
        /// </summary>
        private void Awake()
        {
            _horizontalStuckTime = 0.0f;
            _verticalStuckTime = 0.0f;

            _rb = GetComponent<Rigidbody2D>();
            _ball = GetComponent<Ball>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (Time.timeScale == 0.0f || _ball.IsAttached())
            {
                return;
            }
            CheckHorizontal();
            CheckVertical();
        }

        /// <summary>
        /// Check if we're stuck horizontally. In this case, there's no vertical
        /// component to the velocity (velocity.y)
        /// </summary>
        private void CheckHorizontal()
        {
            _rbVelocityX = Math.Abs(_rb.velocity.x);
            if (_rbVelocityX < horizontalVelocityThreshold)
            {
                _horizontalStuckTime += Time.deltaTime;
            }
            else
            {
                _horizontalStuckTime = 0.0f;
            }

            if (_horizontalStuckTime > stuckTimeBeforeIntervention)
            {
                Debug.Log("Detected stuck horizontal. Nudging left...");
                Nudge(Vector2.left);
            }
        }

        /// <summary>
        /// Check if we're stuck vertically. In this case, there's no horizontal
        /// component to the velocity (velocity.x)
        /// If we are, nudge left
        /// </summary>
        private void CheckVertical()
        {
            _rbVelocityY = Math.Abs(_rb.velocity.y);

            if (_rbVelocityY < verticalVelocityThreshold)
            {
                _verticalStuckTime += Time.deltaTime;
            }
            else
            {
                _verticalStuckTime = 0.0f;
            }

            if (_verticalStuckTime > stuckTimeBeforeIntervention)
            {
                Debug.Log("Detected stuck horizontal. Nudging up...");
                Nudge(Vector2.up);
            }
        }

        /// <summary>
        /// Check if we're stuck horizontally. If we are, nudge up.
        /// </summary>
        /// <param name="direction"></param>
        private void Nudge(Vector2 direction)
        {
            _rb.velocity += (direction * nudgeForce);
        }
    }
}

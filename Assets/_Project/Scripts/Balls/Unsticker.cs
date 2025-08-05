using System;
using DaftAppleGames.RetroRacketRevolution.Balls;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.Balls
{
    public class Unsticker : MonoBehaviour
    {
        [BoxGroup("Settings")] [SerializeField] private float stuckTimeBeforeIntervention = 3.0f;
        [BoxGroup("Settings")] [SerializeField] private float verticalVelocityThreshold = 0.01f;
        [BoxGroup("Settings")] [SerializeField] private float horizontalVelocityThreshold = 0.01f;
        [BoxGroup("Settings")] [SerializeField] private float nudgeForce = 1.0f;

        [BoxGroup("Debug")] [SerializeField] private float _rbVelocityX;
        [BoxGroup("Debug")] [SerializeField] private float _rbVelocityY;


        [BoxGroup("Debug")] [SerializeField] private float _horizontalStuckTime;
        [BoxGroup("Debug")] [SerializeField] private float _verticalStuckTime;

        private Ball _ball;

        private Rigidbody _rb;

        /// <summary>
        /// Initialise this component
        /// </summary>
        private void Awake()
        {
            _horizontalStuckTime = 0.0f;
            _verticalStuckTime = 0.0f;

            _rb = GetComponent<Rigidbody>();
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
            _rbVelocityX = Math.Abs(_rb.linearVelocity.x);
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
                Nudge(Vector3.left);
            }
        }

        /// <summary>
        /// Check if we're stuck vertically. In this case, there's no horizontal
        /// component to the velocity (velocity.x)
        /// If we are, nudge left
        /// </summary>
        private void CheckVertical()
        {
            _rbVelocityY = Math.Abs(_rb.linearVelocity.y);

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
                Nudge(Vector3.up);
            }
        }

        /// <summary>
        /// Check if we're stuck horizontally. If we are, nudge up.
        /// </summary>
        private void Nudge(Vector3 direction)
        {
            _rb.linearVelocity += (direction * nudgeForce);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution
{
    public enum VortexDirection { Outward, Inward, Both}
    
    public class Disruptor : MonoBehaviour
    {
        [BoxGroup("Settings")] public VortexDirection direction;
        [BoxGroup("Settings")] public float delayBetweenChange = 5.0f;
        [BoxGroup("Settings")] public float rotateSpeed = 1.0f;
        [BoxGroup("Settings")] public float disruptiveForce = 1.0f;
        [BoxGroup("Colors")] public Color inwardColour = Color.blue;
        [BoxGroup("Colors")] public Color outwardColour = Color.blue;

        private VortexDirection _currentDirection;
        private float _nextDirectionChangeTime;

        private SpriteRenderer _spriteRenderer;

        /// <summary>
        /// Initialise this component
        /// </summary>
        private void Awake()
        {
            _currentDirection = direction;
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            UpdateColor();
        }

        /// <summary>
        /// Handle the ball passing through
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerStay2D(Collider2D other)
        {
            if(other.gameObject.CompareTag("Ball"))
            {
                ApplyForce(other.gameObject);
            }
        }

        /// <summary>
        /// Apply force to the ball
        /// </summary>
        /// <param name="ballGameObject"></param>
        private void ApplyForce(GameObject ballGameObject)
        {
            // Get the ball RigidBody           
            Rigidbody2D rb = ballGameObject.GetComponent<Rigidbody2D>();

            if (_currentDirection == VortexDirection.Inward)
            {
                // rb.velocity *= RotateLeft(rb.velocity);
                rb.AddForce(rb.transform.right * disruptiveForce, ForceMode2D.Impulse);
            }
            else
            {
                // rb.velocity *= RotateRight(rb.velocity);
                rb.AddForce(-rb.transform.right * disruptiveForce, ForceMode2D.Impulse);
            }
        }

        /// <summary>
        /// Rotate to left by one degree
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        private Vector2 RotateLeft(Vector2 vector)
        {
            return new Vector2(vector.y * -0.1f, vector.x).normalized;
        }

        /// <summary>
        /// Rotate to right by one degree
        /// </summary>
        private Vector2 RotateRight(Vector2 vector)
        {
            return new Vector2(vector.y, vector.x * -0.1f).normalized;
        }

        /// <summary>
        /// Rotate the disruptor
        /// </summary>
        private void Update()
        {
            switch (_currentDirection)
            {
                case VortexDirection.Outward:
                    transform.Rotate(0, 0, rotateSpeed);
                    break;
                default:
                    transform.Rotate(0, 0, -rotateSpeed);
                    break;
            }

            if (IsTimeToChangeDirection())
            {
                ChangeDirection();
            }
        }

        /// <summary>
        /// Change direction
        /// </summary>
        private void ChangeDirection()
        {
            _currentDirection = _currentDirection == VortexDirection.Outward ? VortexDirection.Inward : VortexDirection.Outward;
            UpdateColor();
            _nextDirectionChangeTime = Time.time + delayBetweenChange;
        }

        /// <summary>
        /// Sets the color of the disruptor, depending on current direction
        /// </summary>
        private void UpdateColor()
        {
            _spriteRenderer.color = _currentDirection == VortexDirection.Inward ? inwardColour : outwardColour;
        }

        /// <summary>
        /// Check if time to change direction
        /// </summary>
        /// <returns></returns>
        private bool IsTimeToChangeDirection()
        {
            if (direction != VortexDirection.Both)
            {
                return false;
            }

            if (Time.time > _nextDirectionChangeTime)
            {
                return true;
            }

            return false;
        }
    }
}

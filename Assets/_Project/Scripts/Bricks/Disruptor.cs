using Sirenix.OdinInspector;
using SpaceGraphicsToolkit.Ring;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.Bricks
{
    public enum VortexDirection
    {
        Outward,
        Inward,
        Both
    }

    public class Disruptor : MonoBehaviour
    {
        [BoxGroup("Settings")] public VortexDirection direction;
        [BoxGroup("Settings")] public float delayBetweenChange = 5.0f;
        [BoxGroup("Settings")] public float rotateSpeed = 1.0f;
        [BoxGroup("Settings")] public float disruptiveForce = 1.0f;
        [BoxGroup("Colors")] public Color inwardColour = Color.blue;
        [BoxGroup("Colors")] public Color outwardColour = Color.red;

        private VortexDirection _currentDirection;
        private float _nextDirectionChangeTime;
        private SgtRing _accretionRing;

        /// <summary>
        /// Initialise this component
        /// </summary>
        private void Awake()
        {
            _accretionRing = GetComponentInChildren<SgtRing>();
            _currentDirection = direction;
            UpdateColor();
        }

        /// <summary>
        /// Handle the ball passing through
        /// </summary>
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Ball"))
            {
                ApplyForce(other.gameObject);
            }
        }

        /// <summary>
        /// Apply force to the ball
        /// </summary>
        private void ApplyForce(GameObject ballGameObject)
        {
            // Get the ball RigidBody           
            Rigidbody rb = ballGameObject.GetComponent<Rigidbody>();

            if (_currentDirection == VortexDirection.Inward)
            {
                // rb.velocity *= RotateLeft(rb.velocity);
                rb.AddForce(rb.transform.right * disruptiveForce, ForceMode.Impulse);
            }
            else
            {
                // rb.velocity *= RotateRight(rb.velocity);
                rb.AddForce(-rb.transform.right * disruptiveForce, ForceMode.Impulse);
            }
        }

        /// <summary>
        /// Rotate the disruptor
        /// </summary>
        private void Update()
        {
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
            _currentDirection = _currentDirection == VortexDirection.Outward
                ? VortexDirection.Inward
                : VortexDirection.Outward;
            UpdateColor();
            _nextDirectionChangeTime = Time.time + delayBetweenChange;
        }

        /// <summary>
        /// Sets the color of the disruptor, depending on current direction
        /// </summary>
        private void UpdateColor()
        {
            if (_currentDirection == VortexDirection.Outward)
            {
                _accretionRing.Color = outwardColour;
            }
            else
            {
                _accretionRing.Color = inwardColour;
            }
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
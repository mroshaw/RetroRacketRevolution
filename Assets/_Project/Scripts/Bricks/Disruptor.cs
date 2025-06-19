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


        /// <summary>
        /// Initialise this component
        /// </summary>
        private void Awake()
        {
            _currentDirection = direction;
            UpdateColor();
        }

        /// <summary>
        /// Handle the ball passing through
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerStay(Collider other)
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
        /// Rotate to left by one degree
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        private Vector3 RotateLeft(Vector3 vector)
        {
            return new Vector3(vector.y * -0.1f, vector.x, vector.z).normalized;
        }

        /// <summary>
        /// Rotate to right by one degree
        /// </summary>
        private Vector3 RotateRight(Vector3 vector)
        {
            return new Vector3(vector.y, vector.x * -0.1f, vector.z).normalized;
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
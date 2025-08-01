using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace DaftAppleGames.RetroRacketRevolution.Players
{

    public class PlayerMovement : MonoBehaviour
    {
        [BoxGroup("Movement")] [SerializeField] private float digitalSpeedModified = 2000.0f;
        [BoxGroup("Movement")] [SerializeField] private float analogueSpeedModifier = 200.0f;
        [BoxGroup("Movement")] [SerializeField] private float minX = -200.0f;
        [BoxGroup("Movement")] [SerializeField] private float maxX = 200.0f;

        [BoxGroup("Debug")] [SerializeField] private Vector2 moveVector;
        [BoxGroup("Debug")] [SerializeField] private float horizontal;
        [BoxGroup("Debug")] [SerializeField] private bool fire;
        
        [BoxGroup("Events")] public UnityEvent onMovingLeft;
        [BoxGroup("Events")] public UnityEvent onMovingRight;
        [BoxGroup("Events")] public UnityEvent onStopped;
        public bool ControlsEnabled { get; private set; }

        private float _speed = 1.0f;
        private bool _isMoving;
        private Vector2 _moveVector;
        
        private Rigidbody _rb;

        private float _keyboardSpeedMultiplier;
        private float _dpadSpeedMultiplier;

        private Player _player;
        
        /// <summary>
        /// Init the player controls
        /// </summary>
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _player = GetComponent<Player>();
            ControlsEnabled = true;
            _moveVector =  Vector2.zero;
        }

        /// <summary>
        /// Hide the mouse cursor
        /// </summary>
        private void Start()
        {
            Cursor.visible = false;
        }
        
        internal void ConfigurePlayer(float newMinX, float newMaxX)
        {
            minX = newMinX;
            maxX = newMaxX;
        }

        /// <summary>
        /// Enable controls
        /// </summary>
        public void EnableControls()
        {
            ControlsEnabled = true;
        }

        /// <summary>
        /// Disable controls
        /// </summary>
        public void DisableControls()
        {
            ControlsEnabled = false;
        }

        /// <summary>
        /// Handle settings change to control sensitivity
        /// </summary>
        public void KeyboardSensitivityChanged(float newValue)
        {
            digitalSpeedModified =  newValue;
        }

        /// <summary>
        /// Handle settings change to control sensitivity
        /// </summary>
        public void DpadSensitivityChanged(float newValue)
        {
            digitalSpeedModified = newValue;
        }

        /// <summary>
        /// Determines whether the player can be controlled
        /// </summary>
        private bool CanControl()
        {
            return ControlsEnabled && !_player.Destroyed;
        }

        public void SetMoveVector(Vector2 newMoveVector)
        {
            moveVector = newMoveVector;
            horizontal = moveVector.normalized.x;

            switch (horizontal)
            {
                case 0.0f when _isMoving:
                    _isMoving = false;
                    onStopped.Invoke();
                    break;
                case < 0.0f when !_isMoving:
                    _isMoving = true;
                    onMovingLeft.Invoke();
                    break;
                case > 0.0f when !_isMoving:
                    _isMoving = true;
                    onMovingRight.Invoke();
                    break;
            }
        }
        
        /// <summary>
        /// Reposition if exceeded boundary limits
        /// </summary>
        private bool CheckBoundaries(Vector3 position, out Vector3 newPosition)
        {
            if (position.x < minX)
            {
                newPosition = new Vector3(minX, position.y, position.z);
                return true;
            }

            if (position.x > maxX)
            {
                newPosition = new Vector3(maxX, position.y, position.z);
                return true;
            }
            newPosition = position;
            return false;
        }


        /// <summary>
        /// Check player against screen boundaries
        /// </summary>
        private void Update()
        {
            // Check boundaries
            if (CheckBoundaries(gameObject.transform.localPosition, out Vector3 newPosition))
            {
                gameObject.transform.localPosition = newPosition;
                horizontal = 0;
            }
        }

        /// <summary>
        /// Move the player
        /// </summary>
        private void FixedUpdate()
        {
            /*
            switch (currentControlScheme)
            {
                case "Mouse":
                    _rb.linearVelocity = Vector3.zero;
                    return;
                case "Keyboard":
                    multiplier = _keyboardSpeedMultiplier;
                    break;
                case "Gamepad":
                    multiplier = _dpadSpeedMultiplier;
                    break;
            }
            */
            _rb.linearVelocity = Vector2.right * (horizontal * _speed * digitalSpeedModified);
        }
    }
}
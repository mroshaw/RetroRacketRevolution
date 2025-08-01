using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace DaftAppleGames.RetroRacketRevolution.Players
{
    public enum ControlScheme { Keyboard, Mouse, Gamepad, Touchpad }

    public class PlayerControls : MonoBehaviour
    {
        [BoxGroup("Movement")] [SerializeField] private float defaultKeyboardSpeedMultiplier = 2000.0f;
        [BoxGroup("Movement")] [SerializeField] private float defaultDpadSpeedMultiplier = 200.0f;
        [BoxGroup("Movement")] [SerializeField] private float minX = -200.0f;
        [BoxGroup("Movement")] [SerializeField] private float maxX = 200.0f;

        [BoxGroup("Debug")] [SerializeField] private string currentControlScheme;
        [BoxGroup("Debug")] [SerializeField] private Vector2 moveVector;
        [BoxGroup("Debug")] [SerializeField] private float horizontal;
        [BoxGroup("Debug")] [SerializeField] private bool fire;
        [BoxGroup("Debug")] [SerializeField] private float debugXLastFrame;
        [BoxGroup("Debug")] [SerializeField] private float debugCurrentX;
        
        [BoxGroup("Events")] public UnityEvent FireButtonPressedEvent;
        [BoxGroup("Events")] public UnityEvent FireButtonReleasedEvent;
        [BoxGroup("Events")] public UnityEvent MovingLeftEvent;
        [BoxGroup("Events")] public UnityEvent MovingRightEvent;
        [BoxGroup("Events")] public UnityEvent StoppedEvent;

        public bool ControlsEnabled { get; private set; }

        private float _speed = 1.0f;
        private float _xLastFrame;
        private bool _isMoving;
        
        private PlayerInput _playerInput;
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
            _playerInput = GetComponent<PlayerInput>();
            _player = GetComponent<Player>();
            currentControlScheme = _playerInput.currentControlScheme;

            _keyboardSpeedMultiplier = defaultKeyboardSpeedMultiplier;
            _dpadSpeedMultiplier = defaultDpadSpeedMultiplier;

            ControlsEnabled = true;
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
            _keyboardSpeedMultiplier = defaultKeyboardSpeedMultiplier * newValue;
        }

        /// <summary>
        /// Handle settings change to control sensitivity
        /// </summary>
        public void DpadSensitivityChanged(float newValue)
        {
            _dpadSpeedMultiplier = defaultDpadSpeedMultiplier * newValue;
        }

        /// <summary>
        /// Determines whether the player can be controlled
        /// </summary>
        private bool CanControl()
        {
            return ControlsEnabled && !_player.Destroyed;
        }
        
        /// <summary>
        /// Handle the "Move" message from Player Controls
        /// </summary>
        public void OnMove(InputAction.CallbackContext context)
        {
            if (!CanControl())
            {
                return;
            }
            moveVector = context.ReadValue<Vector2>();
            horizontal = moveVector.normalized.x;

            switch (horizontal)
            {
                case 0.0f when _isMoving:
                    _isMoving = false;
                    StoppedEvent.Invoke();
                    break;
                case < 0.0f when !_isMoving:
                    _isMoving = true;
                    MovingLeftEvent.Invoke();
                    break;
                case > 0.0f when !_isMoving:
                    _isMoving = true;
                    MovingRightEvent.Invoke();
                    break;
            }
        }

        /// <summary>
        /// Handles the "MoveAnalogue" message from Player Controls
        /// </summary>
        public void OnMoveAnalogue(InputAction.CallbackContext context)
        {
            if (!CanControl())
            {
                return;
            }
            
            if (_playerInput.currentControlScheme == "Mouse")
            {
                Vector2 mousePosition = context.ReadValue<Vector2>();
                Vector2 moveVector = Camera.main.ScreenToWorldPoint(mousePosition);
                Vector2 position = new Vector2(moveVector.x, gameObject.transform.position.y);
                
                if (CheckBoundaries(position, out Vector3 newPosition))
                {
#if !UNITY_EDITOR
                    if (ControlsEnabled && (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.LinuxPlayer))
                    {
                        Mouse.current.WarpCursorPosition(Camera.main.WorldToScreenPoint(newPosition));
                    }
#endif
                    gameObject.transform.position = newPosition;

                }
                else
                {
                    gameObject.transform.position = position;
                }
            }
            else
            {
                // Gamepad
                horizontal = context.ReadValue<Vector2>().normalized.x;
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
        /// Handle the "OnControlsChanged" message
        /// </summary>
        private void OnControlsChanged()
        {
            if (_playerInput == null)
            {
                _playerInput = GetComponent<PlayerInput>();
            }
            Debug.Log($"Control Scheme Changed: {_playerInput.gameObject.name} to {_playerInput.currentControlScheme}");
        }

        /// <summary>
        /// Handle the "Fire" message from Player Controls
        /// </summary>
        public void OnFire(InputAction.CallbackContext context)
        {
            if (!CanControl())
            {
                return;
            }

            if (context.started)
            {
                FireButtonPressedEvent.Invoke();
                return;
            }

            if (context.canceled)
            {
                FireButtonReleasedEvent.Invoke();
                return;
            }
        }

        /// <summary>
        /// Check player against screen boundaries
        /// </summary>
        private void Update()
        {
            // DEBUG
            debugXLastFrame = _xLastFrame;
            debugCurrentX = transform.position.x;

            // Check boundaries
            if (CheckBoundaries(gameObject.transform.localPosition, out Vector3 newPosition))
            {
                gameObject.transform.localPosition = newPosition;
                horizontal = 0;
            }
            _xLastFrame = transform.position.x;

            if (_playerInput.currentControlScheme == "Mouse")
            {
                return;
            }

        }

        /// <summary>
        /// Move the player
        /// </summary>
        private void FixedUpdate()
        {
            float multiplier = 1.0f;

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
            _rb.linearVelocity = Vector2.right * horizontal * _speed * multiplier;
        }
    }
}
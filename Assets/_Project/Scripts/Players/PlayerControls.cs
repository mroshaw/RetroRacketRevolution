using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace DaftAppleGames.RetroRacketRevolution.Players
{
    public enum ControlScheme { Keyboard, Mouse, Gamepad, Touchpad }

    public class PlayerControls : MonoBehaviour
    {
        [BoxGroup("Movement")] public float defaultKeyboardSpeedMultiplier = 2000.0f;
        [BoxGroup("Movement")] public float defaultDpadSpeedMultiplier = 200.0f;
        [BoxGroup("Movement")] public float minX = -200.0f;
        [BoxGroup("Movement")] public float maxX = 200.0f;

        [BoxGroup("Debug")][SerializeField] private string _currentControlScheme;
        [BoxGroup("Debug")][SerializeField] private Vector2 _moveVector;
        [BoxGroup("Debug")][SerializeField] private float _horizontal;
        [BoxGroup("Debug")][SerializeField] private bool _fire;
        [BoxGroup("Debug")]public float debugXLastFrame;
        [BoxGroup("Debug")]public float debugCurrentX;
        
        [BoxGroup("Events")] public UnityEvent FireButtonEvent;
        [BoxGroup("Events")] public UnityEvent MovingLeftEvent;
        [BoxGroup("Events")] public UnityEvent MovingRightEvent;
        [BoxGroup("Events")] public UnityEvent StoppedEvent;

        public bool ControlsEnabled { get; private set; }

        private float _speed = 1.0f;
        private float _xLastFrame;


        private PlayerInput _playerInput;
        private Rigidbody2D _rb;

        private Vector2 noVelocity = new Vector2(0, 0);

        private float _keyboardSpeedMultiplier;
        private float _dpadSpeedMultiplier;


        /// <summary>
        /// Init the player controls
        /// </summary>
        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _playerInput = GetComponent<PlayerInput>();
            _currentControlScheme = _playerInput.currentControlScheme;

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

        /// <summary>
        /// Enable controls
        /// </summary>
        public void EnableControls()
        {
            ControlsEnabled = true;
            // Debug.Log($"{this.GetComponent<Player>().gameObject.name} controls enabled.");
        }

        /// <summary>
        /// Disable controls
        /// </summary>
        public void DisableControls()
        {
            ControlsEnabled = false;
            // Debug.Log($"{this.GetComponent<Player>().gameObject.name} controls disabled.");
        }

        /// <summary>
        /// Handle settings change to control sensitivity
        /// </summary>
        /// <param name="newValue"></param>
        public void KeyboardSensitivityChanged(float newValue)
        {
            _keyboardSpeedMultiplier = defaultKeyboardSpeedMultiplier * newValue;
        }

        /// <summary>
        /// Handle settings change to control sensitivity
        /// </summary>
        /// <param name="newValue"></param>
        public void DpadSensitivityChanged(float newValue)
        {
            _dpadSpeedMultiplier = defaultDpadSpeedMultiplier * newValue;
        }

        /// <summary>
        /// Handle the "Move" message from Player Controls
        /// </summary>
        /// <param name="value"></param>
        private void OnMove(InputValue value)
        {
            _moveVector = value.Get<Vector2>();
            _horizontal = _moveVector.normalized.x;
        }

        /// <summary>
        /// Handles the "MoveAnalogue" message from Player Controls
        /// </summary>
        /// <param name="value"></param>
        private void OnMoveAnalogue(InputValue value)
        {
            if (_playerInput.currentControlScheme == "Mouse")
            {
                Vector2 mousePosition = value.Get<Vector2>();
                Vector2 moveVector = Camera.main.ScreenToWorldPoint(mousePosition);
                Vector2 position = new Vector2(moveVector.x, gameObject.transform.position.y);
                
                if (CheckBoundaries(position, out Vector2 newPosition))
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
                _horizontal = value.Get<Vector2>().normalized.x;
            }
        }

        /// <summary>
        /// Reposition if exceeded boundary limits
        /// </summary>
        private bool CheckBoundaries(Vector2 position, out Vector2 newPosition)
        {
            if (position.x < minX)
            {
                newPosition = new Vector2(minX, position.y);
                return true;
            }

            if (position.x > maxX)
            {
                newPosition = new Vector2(maxX, position.y);
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
        private void OnFire()
        {
            if (!ControlsEnabled)
            {
                return;
            }
            FireButtonEvent.Invoke();
        }

        /// <summary>
        /// Check player against screen boundaries
        /// </summary>
        private void Update()
        {
            // DEBUG
            debugXLastFrame = _xLastFrame;
            debugCurrentX = transform.position.x;

            // Determine if we're moving
            if (transform.position.x > _xLastFrame)
            {
                MovingRightEvent.Invoke();
            }
            else if (transform.position.x < _xLastFrame)
            {
                MovingLeftEvent.Invoke();
            }
            else
            {
                StoppedEvent.Invoke();
            }

            _xLastFrame = transform.position.x;

            if (_playerInput.currentControlScheme == "Mouse")
            {
                return;
            }
            // Check boundaries
            if (CheckBoundaries(gameObject.transform.localPosition, out Vector2 newPosition))
            {
               gameObject.transform.localPosition = newPosition;
            }
        }

        /// <summary>
        /// Move the player
        /// </summary>
        private void FixedUpdate()
        {
            float multiplier = 1.0f;

            switch (_currentControlScheme)
            {
                case "Mouse":
                    _rb.linearVelocity = noVelocity;
                    return;
                case "Keyboard":
                    multiplier = _keyboardSpeedMultiplier;
                    break;
                case "Gamepad":
                    multiplier = _dpadSpeedMultiplier;
                    break;
            }
            _rb.linearVelocity = Vector2.right * _horizontal * _speed * multiplier;
        }
    }
}

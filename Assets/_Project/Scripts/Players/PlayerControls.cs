using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace DaftApplesGames.RetroRacketRevolution.Players
{
    public class PlayerControls : MonoBehaviour
    {
        [BoxGroup("Movement")] public float keyboardSpeedMultiplier = 1000.0f;
        [BoxGroup("Movement")] public float dpadSpeedMultiplier = 100.0f;
        [BoxGroup("Movement")] public float minX = -200.0f;
        [BoxGroup("Movement")] public float maxX = 200.0f;

        public bool ControlsEnabled { get; private set; }

        private float _speed = 1.0f;

        [BoxGroup("Debug")]
        [SerializeField]
        private string _currentControlScheme;

        [BoxGroup("Debug")]
        [SerializeField]
        private Vector2 _moveVector;
        [BoxGroup("Debug")]
        [SerializeField]
        private float _horizontal;
        [BoxGroup("Debug")]
        [SerializeField]
        private bool _fire;

        private PlayerInput _playerInput;
        private Rigidbody2D _rb;

        private Vector2 noVelocity = new Vector2(0, 0);
        
        [BoxGroup("Events")] public UnityEvent FireButtonEvent;

        /// <summary>
        /// Init the player controls
        /// </summary>
        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _playerInput = GetComponent<PlayerInput>();
            _currentControlScheme = _playerInput.currentControlScheme;
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
                float x = moveVector.x;
                if (moveVector.x < minX)
                {
                    x = minX;
                }
                else if (moveVector.x > maxX)
                {
                    x = maxX;
                }
                Vector2 position = new Vector2(x, 0);

                gameObject.transform.localPosition = position;
#if !UNITY_EDITOR
                if (ControlsEnabled && (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.LinuxPlayer))
                {
                    Mouse.current.WarpCursorPosition(Camera.main.WorldToScreenPoint(position));
                }
#endif
            }
            else
            {
                // Gamepad
                _horizontal = value.Get<Vector2>().normalized.x;
            }
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
            Debug.Log($"Control Scheme Changed: {_playerInput.currentControlScheme}");
        }

        /// <summary>
        /// Handle the "Fire" message from Player Controls
        /// </summary>
        /// <param name="value"></param>
        private void OnFire()
        {
            if (!ControlsEnabled)
            {
                return;
            }
            FireButtonEvent.Invoke();
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
                    _rb.velocity = noVelocity;
                    return;
                case "Keyboard":
                    multiplier = keyboardSpeedMultiplier;
                    break;
                case "Gamepad":
                    multiplier = dpadSpeedMultiplier;
                    break;
            }

            _rb.velocity = Vector2.right * _horizontal * _speed * multiplier;

            if (transform.localPosition.x < minX)
            {
                transform.localPosition = new Vector2(minX, 0);
            }
            else if (transform.localPosition.x > maxX)
            {
                transform.localPosition = new Vector2(maxX, 0);
            }
        }
    }
}

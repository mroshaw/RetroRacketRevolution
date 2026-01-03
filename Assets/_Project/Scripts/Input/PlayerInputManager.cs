using DaftAppleGames.RetroRacketRevolution.Game;
using DaftAppleGames.RetroRacketRevolution.Players;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DaftAppleGames.RetroRacketRevolution.Input
{
    public class PlayerInputManager : InputManager
    {
        private Player _player;
        private PlayerMovement _playerMovement;

        private InputAction MovementInputAction { get; set; }
        private InputAction AnalogueMovementInputAction { get; set; }
        private InputAction FireInputAction { get; set; }

        private bool _isGameBusy;

        private bool CanMove => !_player.Destroyed;

        private void Awake()
        {
            _player = GetComponent<Player>();
            _playerMovement = GetComponent<PlayerMovement>();
        }

        protected override void InitInput()
        {
            base.InitInput();

            MovementInputAction = InputActionsAsset.FindAction("Move");

            if (MovementInputAction != null)
            {
                MovementInputAction.started += OnMove;
                MovementInputAction.canceled += OnStop;
                MovementInputAction?.Enable();
            }
            else
            {
                Debug.LogError("No Move action found!");
            }

            AnalogueMovementInputAction = InputActionsAsset.FindAction("MoveAnalogue");

            if (AnalogueMovementInputAction != null)
            {
                AnalogueMovementInputAction?.Enable();
            }
            else
            {
                Debug.LogError("No MoveAnalogue action found!");
            }

            FireInputAction = InputActionsAsset.FindAction("Fire");
            if (FireInputAction != null)
            {
                FireInputAction.started += OnFire;
                FireInputAction.canceled += OnFire;
                FireInputAction.Enable();
            }
            else
            {
                Debug.LogError("No Fire action found!");
            }
        }

        protected override void DeInitInput()
        {
            base.DeInitInput();

            // Unsubscribe from input action events and disable input actions
            if (MovementInputAction != null)
            {
                MovementInputAction.Disable();
                MovementInputAction = null;
            }

            if (FireInputAction != null)
            {
                FireInputAction.Disable();
                FireInputAction = null;
            }
        }

        /// <summary>
        /// Link to Game Manager event to block inputs when game UI is busy
        /// </summary>
        public void SetGameBusy()
        {
            _isGameBusy = true;
        }

        /// <summary>
        /// Link to Game Manager event to resume input when game UI is ready
        /// </summary>
        public void SetGameReady()
        {
            _isGameBusy = false;
        }

        private void OnFire(InputAction.CallbackContext context)
        {
            if (context.started && !_isGameBusy)
            {
                _player.BeginFiring();
                return;
            }

            if (context.canceled)
            {
                _player.EndFiring();
                return;
            }
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            Vector2 movement = MovementInputAction?.ReadValue<Vector2>() ??
                               AnalogueMovementInputAction?.ReadValue<Vector2>() ?? Vector2.zero;
            _playerMovement.SetMoveVector(movement);
        }

        private void OnStop(InputAction.CallbackContext context)
        {
            _playerMovement.SetMoveVector(Vector2.zero);
        }

        /// <summary>
        /// Polls movement InputAction (if any).
        /// Return its current value or zero if no valid InputAction found.
        /// </summary>
        private Vector2 GetMovementInput()
        {
            return MovementInputAction?.ReadValue<Vector2>() ??
                   AnalogueMovementInputAction?.ReadValue<Vector2>() ?? Vector2.zero;
        }
    }
}
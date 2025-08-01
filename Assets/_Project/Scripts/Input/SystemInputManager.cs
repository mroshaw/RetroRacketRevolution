using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace DaftAppleGames.Input
{
    public class SystemInputManager : InputManager
    {
        [BoxGroup("Events")] public UnityEvent onPauseToggled;
        
        private InputAction PauseInputAction { get; set; }
        
        protected override void InitInput()
        {
            base.InitInput();
            // Setup Pause input action handlers
            PauseInputAction = InputActionsAsset.FindAction("Pause");
            if (PauseInputAction != null)
            {
                PauseInputAction.performed += OnPause;
                PauseInputAction.Enable();
            }
            else
            {
                Debug.LogError("No Pause action found!");
            }
        }

        protected override void DeInitInput()
        {
            base.DeInitInput();
            // Unsubscribe from input action events and disable input actions
            if (PauseInputAction != null)
            {
                PauseInputAction.Disable();
                PauseInputAction = null;
            }
        }

        private void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                onPauseToggled?.Invoke();
            }
        }
    }
}

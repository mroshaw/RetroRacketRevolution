using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

namespace DaftAppleGames.RetroRacketRevolution.Input
{
    public abstract class InputManager : MonoBehaviour
    {
        [BoxGroup("Input")] [SerializeField] private InputActionAsset inputActionsAsset;
        protected InputActionAsset InputActionsAsset => inputActionsAsset;

        private void OnEnable()
        {
            InitInput();
        }

        private void OnDisable()
        {
            DeInitInput();
        }

        protected virtual void InitInput()
        {
            if (!inputActionsAsset)
            {
                Debug.LogError("No input asset found!");
                return;
            }
        }

        protected virtual void DeInitInput()
        {
            if (!inputActionsAsset)
            {
                Debug.LogError("No input asset found!");
                return;
            }
        }
    }
}
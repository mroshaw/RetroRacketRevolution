using DaftAppleGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace DaftAppleGames.RetroRacketRevolution.Input
{
    public class CheatsInputManager : InputManager
    {
        private InputAction LaserP1InputAction { get; set; }
        private InputAction SpawnWinLevelInputAction { get; set; }

        private Cheats _cheatManager;

        private void Awake()
        {
            _cheatManager = GetComponent<Cheats>();
        }

        protected override void InitInput()
        {
            base.InitInput();

            LaserP1InputAction = InputActionsAsset.FindAction("CheatLaserP1");
            if (LaserP1InputAction != null)
            {
                LaserP1InputAction.performed += LaserP1;
                LaserP1InputAction.Enable();
            }

            SpawnWinLevelInputAction = InputActionsAsset.FindAction("CheatSpawnWinLevel");
            if (SpawnWinLevelInputAction != null)
            {
                SpawnWinLevelInputAction.performed += SpawnWinLevel;
                SpawnWinLevelInputAction.Enable();
            }
        }

        protected override void DeInitInput()
        {
            base.DeInitInput();
            // Unsubscribe from input action events and disable input actions
            if (LaserP1InputAction != null)
            {
                LaserP1InputAction.Disable();
                LaserP1InputAction = null;
            }

            if (SpawnWinLevelInputAction != null)
            {
                SpawnWinLevelInputAction.Disable();
                SpawnWinLevelInputAction = null;
            }
        }

        private void LaserP1(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _cheatManager.LaserP1();
            }
        }

        private void SpawnWinLevel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _cheatManager.SpawnWinLevel();
            }
        }
    }
}
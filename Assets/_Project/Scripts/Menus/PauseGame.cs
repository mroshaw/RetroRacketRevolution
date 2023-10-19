using DaftApplesGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace DaftApplesGames.RetroRacketRevolution.Menus
{
    public class PauseGame : WindowBase
    {
        [FoldoutGroup("Events")] public UnityEvent PauseEvent;
        [FoldoutGroup("Events")] public UnityEvent ContinueEvent;

        private GamePlayerControls _playerControls;
        private InputAction _pauseAction;
        [SerializeField]
        private bool _isPaused;

        private bool _isPauseWindowOpen;

        /// <summary>
        /// Enable input
        /// </summary>
        private void OnEnable()
        {
            _pauseAction = _playerControls.Player.Pause;
            _pauseAction.Enable();
            _pauseAction.performed += TogglePause;
        }

        /// <summary>
        /// Disable input
        /// </summary>
        private void OnDisable()
        {
            _pauseAction.Disable();
        }

        /// <summary>
        /// Set up inputs
        /// </summary>
        public override void Awake()
        {
            _playerControls = new GamePlayerControls();
            _isPaused = false;
            _isPauseWindowOpen = false;
            base.Awake();
        }

        /// <summary>
        /// Init the component
        /// </summary>
        private void Start()
        {
            Hide();
        }

        /// <summary>
        /// Toggles the pause state
        /// </summary>
        /// <param name="context"></param>
        public void TogglePause(InputAction.CallbackContext context)
        {
            if (!_isPaused)
            {
                Show();
                Pause();
                return;
            }
            if (_isPaused && _isPauseWindowOpen)
            {
                Continue();
                Hide();
            }
        }

        /// <summary>
        /// Show the pause window
        /// </summary>
        public override void Show()
        {
            base.Show();
            _isPauseWindowOpen = true;
        }

        /// <summary>
        /// Hide the pause window
        /// </summary>
        public override void Hide()
        {
            base.Hide();
            _isPauseWindowOpen = false;
        }

        /// <summary>
        /// Handle Exit to Desktop button click
        /// </summary>
        public void ExitToDesktopClicked()
        {
            Application.Quit();
        }

        /// <summary>
        /// Pause the game
        /// </summary>
        private void Pause()
        {
            if (_isPaused)
                return;

            Time.timeScale = 0.0f;
            _isPaused = true;

            PauseEvent.Invoke();
        }

        /// <summary>
        /// Unpause the game
        /// </summary>
        public void Continue()
        {
            if (!_isPaused)
                return;

            Time.timeScale = 1.0f;
            _isPaused = false;

            ContinueEvent.Invoke();
        }
    }
}

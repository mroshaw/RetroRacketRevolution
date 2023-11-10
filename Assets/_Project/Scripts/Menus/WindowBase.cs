using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DaftAppleGames.RetroRacketRevolution.Menus
{
    public class WindowBase : MonoBehaviour
    {
        [BoxGroup("UI Settings")] public GameObject uiPanelGameObject;
        [BoxGroup("UI Settings")] public bool hidePanelOnAwake = true;
        [BoxGroup("UI Settings")] public GameObject firstSelectedGameObject;
        
        [FoldoutGroup("Events")] public UnityEvent WindowShowEvent;
        [FoldoutGroup("Events")] public UnityEvent WindowHideEvent;

        private CursorLockMode _cursorLockMode;
        private bool _cursorVisible;

        /// <summary>
        /// Initialise this component
        /// </summary>
        public virtual void Awake()
        {
            if (hidePanelOnAwake)
            {
                uiPanelGameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Setup other components
        /// </summary>
        public virtual void Start()
        {
        }

        /// <summary>
        /// Show the window
        /// </summary>
        public virtual void Show()
        {
            uiPanelGameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(firstSelectedGameObject);

            // Capture current cursor state
            _cursorLockMode = Cursor.lockState;
            _cursorVisible = Cursor.visible;

            // Show cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            WindowShowEvent.Invoke();
        }

        /// <summary>
        /// Hide the window
        /// </summary>
        public virtual void Hide()
        {
            uiPanelGameObject.SetActive(false);

            // Restore cursor state
            Cursor.lockState = _cursorLockMode;
            Cursor.visible = _cursorVisible;

            WindowHideEvent.Invoke();
        }
    }
}

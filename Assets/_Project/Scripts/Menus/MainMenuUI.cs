using DaftAppleGames.UserInterface.MainMenu;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DaftAppleGames.RetroRacketRevolution.Menus
{
    public class MainMenuUI : MainMenuUiController
    {
        [BoxGroup("UI Settings")] [SerializeField] private Button loadLevelEditorButton;
        [BoxGroup("UI Settings")] [SerializeField] private TextMeshProUGUI versionText;
        [BoxGroup("UI Events")] [SerializeField] private UnityEvent onLevelEditorButtonClickEvent;
        /// <summary>
        /// Setup the Main Menu
        /// </summary>
        public override void Start()
        {
            base.Start();
            // Update the version text
            versionText.text = $"Version: {Application.version}";
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            loadLevelEditorButton?.onClick.AddListener(LevelEditorButtonClick);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            loadLevelEditorButton?.onClick.RemoveListener(LevelEditorButtonClick);
        }
        
        private void LevelEditorButtonClick()
        {
            onLevelEditorButtonClickEvent.Invoke();
        }

    }
}
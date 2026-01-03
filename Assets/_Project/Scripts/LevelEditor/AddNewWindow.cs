using DaftAppleGames.RetroRacketRevolution.Menus;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.LevelEditor
{
    public class AddNewWindow : WindowBase
    {
        // Public serializable properties
        [BoxGroup("Managers")] [SerializeField] private LevelEditorManager levelEditorManager;
        [BoxGroup("UI")] public TMP_InputField encodedLevelText;
        [FoldoutGroup("Button Events")] public UnityEvent<string> addButtonClickedEvent;
        [FoldoutGroup("Button Events")] public UnityEvent backButtonClickedEvent;

        /// <summary>
        /// Clear the UI before showing
        /// </summary>
        public override void Show()
        {
            encodedLevelText.text = "";
            base.Show();
        }

        /// <summary>
        /// Handle the Add button
        /// </summary>
        public void AddButtonHandler()
        {
            if (encodedLevelText.text.Length <= 0)
            {
                return;
            }

            levelEditorManager.LoadLevelByEncodedData(encodedLevelText.text);
            addButtonClickedEvent.Invoke(encodedLevelText.text);
        }

        /// <summary>
        /// Handle the Back button
        /// </summary>
        public void BackButtonHandler()
        {
            Hide();
            backButtonClickedEvent.Invoke();
        }
    }
}
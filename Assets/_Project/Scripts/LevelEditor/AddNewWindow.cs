using DaftAppleGames.RetroRacketRevolution.Menus;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using TMPro;

namespace DaftAppleGames.RetroRacketRevolution.LevelEditor
{
    public class AddNewWindow : WindowBase
    {
        // Public serializable properties
        [BoxGroup("UI")] public TMP_InputField encodedLevelText;
        [FoldoutGroup("Button Events")] public UnityEvent<string> AddButtonClickedEvent;
        [FoldoutGroup("Button Events")] public UnityEvent BackButtonClickedEvent;
        
        #region PublicMethods

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
            AddButtonClickedEvent.Invoke(encodedLevelText.text);
        }

        /// <summary>
        /// Handle the Back button
        /// </summary>
        public void BackButtonHandler()
        {
            BackButtonClickedEvent.Invoke();
        }
        #endregion
    }
}

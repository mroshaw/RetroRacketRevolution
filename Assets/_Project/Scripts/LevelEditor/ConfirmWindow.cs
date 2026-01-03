using DaftAppleGames.RetroRacketRevolution.Menus;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using TMPro;

namespace DaftAppleGames.RetroRacketRevolution.LevelEditor
{
    public class ConfirmWindow : WindowBase
    {
        // Public serializable properties
        [BoxGroup("UI")] public TMP_InputField inputField;
        [FoldoutGroup("Button Events")] public UnityEvent YesButtonClickedEvent;
        [FoldoutGroup("Button Events")] public UnityEvent<string> YesButtonWithInputClickedEvent;
        [FoldoutGroup("Button Events")] public UnityEvent NoButtonClickedEvent;
        [FoldoutGroup("Button Events")] public UnityEvent<string> NoButtonWithInputClickedEvent;

        /// <summary>
        /// Handle the Yes button
        /// </summary>
        public void YesButtonHandler()
        {
            if (inputField == null)
            {
                YesButtonClickedEvent.Invoke();
            }
            else
            {
                YesButtonWithInputClickedEvent.Invoke(inputField.text);
            }
        }

        /// <summary>
        /// Handle the No button
        /// </summary>
        public void NoButtonHandler()
        {
            if (inputField == null)
            {
                NoButtonClickedEvent.Invoke();
            }
            else
            {
                NoButtonWithInputClickedEvent.Invoke(inputField.text);
            }
        }
    }
}
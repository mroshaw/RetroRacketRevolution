using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace DaftApplesGames.RetroRacketRevolution.LevelEditor
{
    public class DeleteWindow : WindowBase
    {
        // Public serializable properties
        [FoldoutGroup("Button Events")] public UnityEvent YesButtonClickedEvent;
        [FoldoutGroup("Button Events")] public UnityEvent NoButtonClickedEvent;
        #region PublicMethods

        /// <summary>
        /// Handle the Yes button
        /// </summary>
        public void YesButtonHandler()
        {
            YesButtonClickedEvent.Invoke();
        }

        /// <summary>
        /// Handle the No button
        /// </summary>
        public void NoButtonHandler()
        {
            NoButtonClickedEvent.Invoke();
        }
        #endregion

    }
}

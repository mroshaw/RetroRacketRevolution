using DaftAppleGames.RetroRacketRevolution.Levels;
using DaftAppleGames.RetroRacketRevolution.Menus;
using DaftAppleGames.RetroRacketRevolution.Utils;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;

namespace DaftAppleGames.RetroRacketRevolution.LevelEditor
{
    public class ShareWindow : WindowBase
    {
        // Public serializable properties
        [BoxGroup("UI")] public TMP_InputField encodedLevelDataText;
        [BoxGroup("UI")] public TMP_InputField playerNameText;
        [BoxGroup("UI")] public Button encodeButton;
        [FoldoutGroup("Button Events")] public UnityEvent BackButtonClickedEvent;
        [FoldoutGroup("Button Events")] public UnityEvent<string> EncodeButtonClickedEvent;
        #region PublicMethods

        /// <summary>
        /// Clear the UI before showing
        /// </summary>
        public override void Show()
        {
            encodedLevelDataText.text = "";
            base.Show();
        }

        /// <summary>
        /// Handle the Back button
        /// </summary>
        public void BackButtonHandler()
        {
            BackButtonClickedEvent.Invoke();
        }

        /// <summary>
        /// Handle the Encode button
        /// </summary>
        public void EncodeButtonHandler()
        {
            if (playerNameText.text.Length > 0)
            {
                EncodeButtonClickedEvent.Invoke(playerNameText.text);
            }
        }

        /// <summary>
        /// Show the dialog with the encoded level data
        /// </summary>
       /// <param name="encodedLevelData"></param>
        public void EncodedDataUpdate(string encodedLevelData)
        {
            encodedLevelDataText.text = encodedLevelData;
            encodedLevelData.CopyToClipboard();
        }

        /// <summary>
        /// Enable the button when the player name is populated
        /// </summary>
        /// <param name="playerName"></param>
        public void EnableEncodeButton(string playerName)
        {
            encodeButton.interactable = playerName.Length > 0;
        }
        #endregion
    }
}

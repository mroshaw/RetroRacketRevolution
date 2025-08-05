using DaftAppleGames.RetroRacketRevolution.Menus;
using DaftAppleGames.RetroRacketRevolution.Utils;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DaftAppleGames.RetroRacketRevolution.LevelEditor
{
    public class ShareWindow : WindowBase
    {
        // Public serializable properties
        [BoxGroup("UI")] [SerializeField] private TMP_InputField encodedLevelDataText;
        [BoxGroup("UI")] [SerializeField] private TMP_InputField playerNameText;
        [BoxGroup("UI")] [SerializeField] private Button encodeButton;
        [FoldoutGroup("Button Events")] public UnityEvent onBackButtonClicked;
        [FoldoutGroup("Button Events")] public UnityEvent<string> onEncodeButtonClicked;

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
            onBackButtonClicked.Invoke();
        }

        /// <summary>
        /// Handle the Encode button
        /// </summary>
        public void EncodeButtonHandler()
        {
            if (playerNameText.text.Length > 0)
            {
                onEncodeButtonClicked.Invoke(playerNameText.text);
            }
        }

        /// <summary>
        /// Show the dialog with the encoded level data
        /// </summary>
        public void EncodedDataUpdate(string encodedLevelData)
        {
            encodedLevelDataText.text = encodedLevelData;
            encodedLevelData.CopyToClipboard();
        }

        /// <summary>
        /// Enable the button when the player name is populated
        /// </summary>
        public void EnableEncodeButton(string playerName)
        {
            encodeButton.interactable = playerName.Length > 0;
        }
    }
}
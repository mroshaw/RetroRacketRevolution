using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DaftApplesGames.RetroRacketRevolution.Menus
{
    public class NameImageTemplate : MonoBehaviour
    {
        [BoxGroup("Settings")] public TextMeshProUGUI entryNameText;
        [BoxGroup("Settings")] public TextMeshProUGUI entryDescriptionText;
        [BoxGroup("Settings")] public Image entryImage;

        /// <summary>
        /// Populate template text
        /// </summary>
        /// <param name="entryNameTextContent"></param>
        /// <param name="entryDescTextContent"></param>
        /// <param name="entryImageContent"></param>
        public void SetEntryContent(string entryNameTextContent, string entryDescTextContent, Sprite entryImageContent)
        {
            entryNameText.text = entryNameTextContent;
            entryDescriptionText.text = entryDescTextContent;
            entryImage.sprite = entryImageContent;
        }
    }
}

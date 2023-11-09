using TMPro;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.Menus
{
    public class NameValueTemplate : MonoBehaviour
    {
        public TextMeshProUGUI entryText;
        public TextMeshProUGUI valueText;

        /// <summary>
        /// Populate template text
        /// </summary>
        /// <param name="entryTextContent"></param>
        /// <param name="valueTextContent"></param>
        public void SetEntryText(string entryTextContent, string valueTextContent)
        {
            entryText.text = entryTextContent;
            valueText.text = valueTextContent;
        }
    }
}

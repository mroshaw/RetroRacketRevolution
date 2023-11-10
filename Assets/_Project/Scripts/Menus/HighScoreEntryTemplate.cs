using TMPro;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.Menus
{
    public class HighScoreEntryTemplate : MonoBehaviour
    {
        public TextMeshProUGUI entryText;
        public TextMeshProUGUI valueText;
        public TextMeshProUGUI difficultyText;
        public TextMeshProUGUI levelsPlayedText;
        public TextMeshProUGUI cheatsUsedText;

        /// <summary>
        /// Populate template text
        /// </summary>
        /// <param name="entryTextContent"></param>
        /// <param name="valueTextContent"></param>
        /// <param name="difficultyContent"></param>
        /// <param name="levelsPlayedContent"></param>
        /// <param name="cheatsUsedContent"></param>
        public void SetEntryText(string entryTextContent, string valueTextContent, string difficultyContent, string levelsPlayedContent, string cheatsUsedContent)
        {
            entryText.text = entryTextContent;
            valueText.text = valueTextContent;
            difficultyText.text = difficultyContent;
            levelsPlayedText.text = levelsPlayedContent;
            cheatsUsedText.text = cheatsUsedContent;
        }
    }
}

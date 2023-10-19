using UnityEngine;
using UnityEngine.UI;

namespace DaftApplesGames.RetroRacketRevolution.Levels
{
    public class ColourButton : MonoBehaviour
    {

        private Color _myColour;

        // Start is called before the first frame update
        private void Awake()
        {
            _myColour = GetComponent<Image>().color;
            Button button = GetComponent<Button>();
            button.onClick.AddListener(ButtonClick);
        }

        /// <summary>
        /// Set the LevelEditor color
        /// </summary>
        private void ButtonClick()
        {
            LevelEditorManager.Instance.CurrColor = _myColour;
            LevelEditorManager.Instance.CurrentColourButton.GetComponent<Image>().color = _myColour;
        }
    }
}

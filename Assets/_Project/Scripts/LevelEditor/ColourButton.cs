using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DaftAppleGames.RetroRacketRevolution.LevelEditor
{
    public class ColourButton : MonoBehaviour
    {
        [FoldoutGroup("Events")] public UnityEvent<Color> ColorButtonClickedEvent;

        private Button _button;
        private Color _myColour;

        // Start is called before the first frame update
        private void Awake()
        {
            _myColour = GetComponent<Image>().color;
            _button = GetComponent<Button>();
            _button.onClick.AddListener(ButtonClick);
        }

        /// <summary>
        /// Set the LevelEditor color
        /// </summary>
        private void ButtonClick()
        {
            ColorButtonClickedEvent?.Invoke(_myColour);
        }
    }
}

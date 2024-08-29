using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DaftAppleGames.RetroRacketRevolution.Menus;

namespace DaftAppleGames.RetroRacketRevolution.LevelEditor
{
    public class ColorPickerWindow : WindowBase
    {
        // Public serializable properties
        [BoxGroup("UI Settings")]
        public Slider redSlider;
        [BoxGroup("UI Settings")]
        public Slider greenSlider;
        [BoxGroup("UI Settings")]
        public Slider blueSlider;
        [BoxGroup("UI Settings")]
        public Image colorPreviewImage;
        [FoldoutGroup("Events")]
        public UnityEvent<Color> ColorChangedEvent;
        public UnityEvent<Color> ColorSelectedEvent;

        [SerializeField]
        private Color _color = Color.white;

        /// <summary>
        /// Handler changes to the red value
        /// </summary>
        /// <param name="redValue"></param>
        public void RedSliderHandler(float redValue)
        {

            Color newColor = new Color(redValue / 255, _color.g, _color.b, 1);
            _color = newColor;
            UpdateColorPreview();
        }

        /// <summary>
        /// Handle changes to the green value
        /// </summary>
        /// <param name="greenValue"></param>
        public void GreenSliderHandler(float greenValue)
        {
            Color newColor = new Color(_color.r, greenValue / 255, _color.b, 1);
            _color = newColor;
            UpdateColorPreview();
        }

        /// <summary>
        /// Handle changes to the blue value
        /// </summary>
        /// <param name="blueValue"></param>
        public void BlueSliderHandler(float blueValue)
        {
            Color newColor = new Color(_color.r, _color.g, blueValue / 255, 1);
            _color = newColor;
            UpdateColorPreview();
        }

        /// <summary>
        /// Handle click to the pick button
        /// </summary>
        public void PickButtonHandler()
        {
            ColorSelectedEvent.Invoke(_color);
        }

        /// <summary>
        /// Update the color preview
        /// </summary>
        private void UpdateColorPreview()
        {
            colorPreviewImage.color = _color;
            ColorChangedEvent.Invoke(_color);
        }
    }
}

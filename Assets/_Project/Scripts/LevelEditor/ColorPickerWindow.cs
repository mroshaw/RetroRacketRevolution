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
        [BoxGroup("Managers")] [SerializeField] private LevelEditorWindow levelEditorWindowManager;
        [BoxGroup("UI Settings")] [SerializeField] private Slider redSlider;
        [BoxGroup("UI Settings")] [SerializeField] private Slider greenSlider;
        [BoxGroup("UI Settings")] [SerializeField] private Slider blueSlider;
        [BoxGroup("UI Settings")] [SerializeField] private Image colorPreviewImage;
        [FoldoutGroup("Events")] public UnityEvent<Color> colorChangedEvent;
        [FoldoutGroup("Events")] public UnityEvent<Color> colorSelectedEvent;

        [SerializeField] private Color _color = Color.white;

        /// <summary>
        /// Handler changes to the red value
        /// </summary>
        public void RedSliderHandler(float redValue)
        {
            Color newColor = new Color(redValue / 255, _color.g, _color.b, 1);
            _color = newColor;
            UpdateColorPreview();
        }

        /// <summary>
        /// Handle changes to the green value
        /// </summary>
        public void GreenSliderHandler(float greenValue)
        {
            Color newColor = new Color(_color.r, greenValue / 255, _color.b, 1);
            _color = newColor;
            UpdateColorPreview();
        }

        /// <summary>
        /// Handle changes to the blue value
        /// </summary>
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
            levelEditorWindowManager.SetBrickColour(_color);
        }

        /// <summary>
        /// Update the color preview
        /// </summary>
        private void UpdateColorPreview()
        {
            colorPreviewImage.color = _color;
            colorChangedEvent.Invoke(_color);
        }
    }
}
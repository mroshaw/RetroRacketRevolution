using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using Slider = UnityEngine.UI.Slider;

namespace DaftAppleGames.RetroRacketRevolution.Menus
{
    public class SliderEx : MonoBehaviour
    {
        // Public serializable properties
        [BoxGroup("General Settings")] public TextMeshProUGUI valueText;
        [BoxGroup("General Settings")] public bool showValue;
        
        // Private fields
        private Slider _slider;

        #region UnityMethods
        /// <summary>
        /// Initialise this component
        /// </summary>   
        private void Awake()
        {
            _slider = GetComponent<Slider>();
            valueText.gameObject.SetActive(showValue);
        }
    
        /// <summary>
        /// Configure the component on Start
        /// </summary>
        private void Start()
        {
            // Set the initial value
            if (showValue)
            {
                UpdateValue(_slider.value);
                _slider.onValueChanged.AddListener(UpdateValue);
            }
        }
        #endregion

	    #region PrivateMethods
        /// <summary>
        /// Update the content of the value text field
        /// </summary>
        /// <param name="value"></param>
        private void UpdateValue(float value)
        {
            if (showValue)
            {
                valueText.text = value.ToString("F1");
            }
        }
	    #endregion
    }
}

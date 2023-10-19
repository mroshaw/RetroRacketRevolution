using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DaftApplesGames.RetroRacketRevolution.Menus
{
    public class TextInputButton : MonoBehaviour
    {
        [FoldoutGroup("Events")]
        public UnityEvent<string> LetterButtonClickedEvent;

        private TextMeshProUGUI _labelText;
   
        /// <summary>
        /// Initialise this component
        /// </summary>
        private void Awake()
        {
            _labelText = GetComponentInChildren<TextMeshProUGUI>();
            GetComponent<Button>().onClick.AddListener(OnClickHandler);
        }

        /// <summary>
        /// Handle the button click
        /// </summary>
        public void OnClickHandler()
        {
            LetterButtonClickedEvent.Invoke(_labelText.text);
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using TMPro;

namespace DaftApplesGames.RetroRacketRevolution.LevelEditor
{
    public class AlertText : MonoBehaviour
    {
        // Public serializable properties
        [BoxGroup("Alerts")] public float alertFadeTime = 2.0f;
        [BoxGroup("Alerts")] public float alertVisibleTime = 0.0f;
        [BoxGroup("Alerts")] public AudioClip alertAudioClip;
        [BoxGroup("Alerts")] public AudioClip errorAudioClip;

        [FoldoutGroup("Events")]
        public UnityEvent PreAlertEvent;
        public UnityEvent PostAlertEvent;

        // Public properties

        // Private fields
        private TextMeshProUGUI _alertText;
        private AudioSource _audioSource;
        private Color _visibleColor;
        private Color _hiddenColor;

        #region UnityMethods

        /// <summary>
        /// Initialise this component
        /// </summary>   
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _alertText = GetComponent<TextMeshProUGUI>();
            Color textColor = _alertText.color;
            _visibleColor = new Color(textColor.r, textColor.g, textColor.b, 1);
            _hiddenColor = new Color(textColor.r, textColor.g, textColor.b, 0);
        }
    
        /// <summary>
        /// Configure the component on Start
        /// </summary>
        private void Start()
        {
            _alertText.text = "";
        }
        #endregion

        #region PublicMethods
        /// <summary>
        /// Display an alert
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isError"></param>
        public void DisplayAlert(string message, bool isError)
        {
            if (isError)
            {
                _audioSource.PlayOneShot(errorAudioClip);
            }
            else
            {
                _audioSource.PlayOneShot(alertAudioClip);
            }
            StopAllCoroutines();
            StartCoroutine(NotifyFade(message));
        }
        #endregion

        #region PrivateMethods
        /// <summary>
        /// Show the current notification queue, fade in and out
        /// </summary>
        /// <returns></returns>
        private IEnumerator NotifyFade(string message)
        {
            _alertText.color = _hiddenColor;
            _alertText.text = message;

            // Fade text in
            float time = 0;
            while (time < alertFadeTime)
            {
                _alertText.color = Color.Lerp(_hiddenColor, _visibleColor, time / alertFadeTime);
                time += Time.deltaTime;
                yield return null;
            }
            _alertText.color = _visibleColor;

            // Wait
            yield return new WaitForSecondsRealtime(alertVisibleTime);

            // Fade out
            time = 0;
            while (time < alertFadeTime)
            {
                _alertText.color = Color.Lerp(_visibleColor, _hiddenColor, time / alertFadeTime);
                time += Time.deltaTime;
                yield return null;
            }
            _alertText.color = _hiddenColor;
            _alertText.text = "";
        }
        #endregion
    }
}

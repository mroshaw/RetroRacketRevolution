using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DaftAppleGames.RetroRacketRevolution.Menus
{
    public class SettingsWindow : WindowBase
    {
        [BoxGroup("UI Settings")] public GameObject settingsPanel;
        [BoxGroup("UI Settings")] public Slider musicVolumeSlider;
        [BoxGroup("UI Settings")] public Slider soundFxVolumeSlider;
        [BoxGroup("UI Settings")] public Toggle retroFxToggle;
        [BoxGroup("UI Settings")] public Slider keyboardSensSlider;
        [BoxGroup("UI Settings")] public Slider dpadSensSlider;

        [FoldoutGroup("Events")] public UnityEvent<float> MusicSliderChangedEvent;
        [FoldoutGroup("Events")] public UnityEvent<float> SoundsFxSliderChangedEvent;
        [FoldoutGroup("Events")] public UnityEvent<bool> RetroFxToggleChangedEvent;
        [FoldoutGroup("Events")] public UnityEvent<float> KeyboardSensSliderChangedEvent;
        [FoldoutGroup("Events")] public UnityEvent<float> DpadSensSliderChangedEvent;

        [FoldoutGroup("Events")] public UnityEvent SaveClickedEvent;

        /// <summary>
        /// Init the component
        /// </summary>
        public override void Awake()
        {
            Hide();
            base.Awake();
        }

        /// <summary>
        /// Allow others to subscribe to the Save / Back / Cancel events
        /// </summary>
        public void SaveClickedProxy()
        {
            SaveClickedEvent.Invoke();
        }

        /// <summary>
        /// Handle music slider volume change
        /// </summary>
        /// <param name="newValue"></param>
        public void MusicSliderChanged(float newValue)
        {
            MusicSliderChangedEvent.Invoke(newValue);
        }

        /// <summary>
        /// Handle SoundFX volume change
        /// </summary>
        /// <param name="newValue"></param>
        public void SoundsFxSliderChanged(float newValue)
        {
            SoundsFxSliderChangedEvent.Invoke(newValue);
        }

        /// <summary>
        /// Handle Retro FX toggle change
        /// </summary>
        /// <param name="newValue"></param>
        public void RetroFxToggleChanged(bool newValue)
        {
            RetroFxToggleChangedEvent.Invoke(newValue);
        }

        /// <summary>
        /// Handle Keyboard Sensitivity change
        /// </summary>
        /// <param name="newValue"></param>
        public void KeyboardSensChanged(float newValue)
        {
            KeyboardSensSliderChangedEvent.Invoke(newValue);
        }

        /// <summary>
        /// Handle Dpad Sensitivity change
        /// </summary>
        /// <param name="newValue"></param>
        public void DpadSensChanged(float newValue)
        {
            DpadSensSliderChangedEvent.Invoke(newValue);
        }
        
        /// <summary>
        /// Updates the UI slider value without triggering events
        /// </summary>
        /// <param name="newValue"></param>
        public void SetMusicVolume(float newValue)
        {
            musicVolumeSlider.SetValueWithoutNotify(newValue);
        }

        /// <summary>
        /// Updates the UI slider value without triggering events
        /// </summary>
        /// <param name="newValue"></param>
        public void SetSoundFxVolume(float newValue)
        {
            soundFxVolumeSlider.SetValueWithoutNotify(newValue);
        }

        /// <summary>
        /// Updates the UI toggle with the value
        /// </summary>
        /// <param name="newValue"></param>
        public void SetRetroFxEnabled(bool newValue)
        {
            retroFxToggle.SetIsOnWithoutNotify(newValue);
        }

        /// <summary>
        /// Updates the UI slider with the value
        /// </summary>
        /// <param name="newValue"></param>
        public void SetKeyboardSen(float newValue)
        {
            keyboardSensSlider.SetValueWithoutNotify(newValue);
        }

        /// <summary>
        /// Updates the UI slider with the value
        /// </summary>
        /// <param name="newValue"></param>
        public void SetDpadSen(float newValue)
        {
            dpadSensSlider.SetValueWithoutNotify(newValue);
        }
    }
}

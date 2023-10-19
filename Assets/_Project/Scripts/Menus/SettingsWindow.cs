using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DaftApplesGames.RetroRacketRevolution.Menus
{
    public class SettingsWindow : WindowBase
    {
        [BoxGroup("UI Settings")] public GameObject settingsPanel;
        [BoxGroup("UI Settings")] public Slider musicVolumeSlider;
        [BoxGroup("UI Settings")] public Slider soundFxVolumeSlider;
        [BoxGroup("UI Settings")] public Toggle retroFxToggle;

        [FoldoutGroup("Events")] public UnityEvent<float> MusicSliderChangedEvent;
        [FoldoutGroup("Events")] public UnityEvent<float> SoundsFxSliderChangedEvent;
        [FoldoutGroup("Events")] public UnityEvent<bool> RetroFxToggleChangedEvent;

        /// <summary>
        /// Init the component
        /// </summary>
        public override void Awake()
        {
            Hide();
            base.Awake();
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

        public void RetroFxToggleChanged(bool newValue)
        {
            RetroFxToggleChangedEvent.Invoke(newValue);
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
    }
}

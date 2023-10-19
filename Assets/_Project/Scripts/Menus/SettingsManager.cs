using RetroAesthetics;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace DaftApplesGames.RetroRacketRevolution.Menus
{
    public class SettingsManager : MonoBehaviour
    {
        [BoxGroup("Settings")] public AudioMixer audioMixer;
        [BoxGroup("Settings")] public Camera _camera;
        [BoxGroup("Defaults")] public float defaultMusicVolume = 1.0f;
        [BoxGroup("Defaults")] public float defaultSoundFxVolume = 1.0f;
        [BoxGroup("Defaults")] public bool defaultRetroFxEnabled = false;


        [FoldoutGroup("Events")] public UnityEvent<float> MusicSettingLoadedEvent;
        [FoldoutGroup("Events")] public UnityEvent<float> SoundFxSettingLoadedEvent;
        [FoldoutGroup("Events")] public UnityEvent<bool> RetroFxLoadedEvent;

        private const string MusicVolumeKey = "MusicVolume";
        private const string SoundFxVolumeKey = "SoundsFxVolume";
        private const string RetroFxEnabledKey = "RetroFxEnabled";

        private float _musicVolume;
        private float _soundFxVolume;
        private bool _retroEffectsEnabled;

        /// <summary>
        /// Init the component
        /// </summary>
        public void Awake()
        {
            LoadSettings();
            SetMusicVolume(_musicVolume);
            SetSoundFxVolume(_musicVolume);
            SetRetroEffects(_retroEffectsEnabled);
        }

        /// <summary>
        /// Save settings to User Prefs
        /// </summary>
        public void SaveSettings()
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, _musicVolume);
            PlayerPrefs.SetFloat(SoundFxVolumeKey, _soundFxVolume);
            PlayerPrefs.SetInt(RetroFxEnabledKey, _retroEffectsEnabled ? 1 : 0);
        }

        /// <summary>
        /// Load settings from User Prefs
        /// </summary>
        private void LoadSettings()
        {
            // Load settings
            _musicVolume = PlayerPrefs.HasKey(MusicVolumeKey) ? PlayerPrefs.GetFloat(MusicVolumeKey) : defaultMusicVolume;
            _soundFxVolume = PlayerPrefs.HasKey(SoundFxVolumeKey) ? PlayerPrefs.GetFloat(SoundFxVolumeKey) : defaultSoundFxVolume;
            _retroEffectsEnabled = PlayerPrefs.HasKey(RetroFxEnabledKey)
                ? PlayerPrefs.GetInt(RetroFxEnabledKey)==1
                : defaultRetroFxEnabled;

            // Invoke events
            MusicSettingLoadedEvent.Invoke(_musicVolume);
            SoundFxSettingLoadedEvent.Invoke(_soundFxVolume);
            RetroFxLoadedEvent.Invoke(_retroEffectsEnabled);

        }

        /// <summary>
        /// Set the Music volume
        /// </summary>
        /// <param name="newValue"></param>
        public void SetMusicVolume(float newValue)
        {
            audioMixer.SetFloat("MusicVolume", newValue);
            _musicVolume = newValue;

        }

        /// <summary>
        /// Set the SoundFxVolume
        /// </summary>
        /// <param name="newValue"></param>
        public void SetSoundFxVolume(float newValue)
        {
            audioMixer.SetFloat("SoundFxVolume", newValue);
            _soundFxVolume = newValue;
        }

        /// <summary>
        /// Set the Retro Effects toggle
        /// </summary>
        /// <param name="newValue"></param>
        public void SetRetroEffects(bool newValue)
        {
            RetroCameraEffect retroFx = _camera.GetComponent<RetroCameraEffect>();
            retroFx.enabled = newValue;

            _retroEffectsEnabled = newValue;
        }
    }
}

using RetroAesthetics;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution.Menus
{
    public class SettingsManager : MonoBehaviour
    {
        [BoxGroup("Settings")] public AudioMixer audioMixer;
        [BoxGroup("Settings")] public Camera _camera;

        [BoxGroup("Defaults")] public float defaultMusicVolume;
        [BoxGroup("Defaults")] public float defaultSoundFxVolume;
        [BoxGroup("Defaults")] public bool defaultRetroFxEnabled;
        [BoxGroup("Defaults")] public float defaultKeyboardSens;
        [BoxGroup("Defaults")] public float defaultDpadSens;

        [FoldoutGroup("Loaded Events")] public UnityEvent<float> MusicSettingLoadedEvent;
        [FoldoutGroup("Loaded Events")] public UnityEvent<float> SoundFxSettingLoadedEvent;
        [FoldoutGroup("Loaded Events")] public UnityEvent<bool> RetroFxLoadedEvent;
        [FoldoutGroup("Loaded Events")] public UnityEvent<float> KeyboardSensLoadedEvent;
        [FoldoutGroup("Loaded Events")] public UnityEvent<float> DpadSensLoadedEvent;

        [FoldoutGroup("Changed Events")] public UnityEvent<float> MusicSettingChangedEvent;
        [FoldoutGroup("Changed Events")] public UnityEvent<float> SoundFxSettingChangedEvent;
        [FoldoutGroup("Changed Events")] public UnityEvent<bool> RetroFxChangedEvent;
        [FoldoutGroup("Changed Events")] public UnityEvent<float> KeyboardSensChangedEvent;
        [FoldoutGroup("Changed Events")] public UnityEvent<float> DpadSensChangedEvent;

        private const string MusicVolumeKey = "MusicVolume";
        private const string SoundFxVolumeKey = "SoundsFxVolume";
        private const string RetroFxEnabledKey = "RetroFxEnabled";
        private const string KeyboardSensKey = "KeyboardSensitivity";
        private const string DpadSensKey = "DpadSensitivity";

        private float _musicVolume;
        private float _soundFxVolume;
        private bool _retroEffectsEnabled;
        private float _keyboardSens;
        private float _dpadSens;

        /// <summary>
        /// Init the component
        /// </summary>
        private void Awake()
        {

        }

        /// <summary>
        /// Configure components
        /// </summary>
        private void Start()
        {
            LoadSettings();
        }

        /// <summary>
        /// Save settings to User Prefs
        /// </summary>
        public void SaveSettings()
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, _musicVolume);
            PlayerPrefs.SetFloat(SoundFxVolumeKey, _soundFxVolume);
            PlayerPrefs.SetInt(RetroFxEnabledKey, _retroEffectsEnabled ? 1 : 0);
            PlayerPrefs.SetFloat(KeyboardSensKey, _keyboardSens);
            PlayerPrefs.SetFloat(DpadSensKey, _dpadSens);
        }

        /// <summary>
        /// Load settings from User Prefs
        /// </summary>
        private void LoadSettings()
        {
            // Load settings
            SetMusicVolume(PlayerPrefs.GetFloat(MusicVolumeKey, defaultMusicVolume));
            SetSoundFxVolume(PlayerPrefs.GetFloat(SoundFxVolumeKey, defaultSoundFxVolume));
            SetRetroEffects(PlayerPrefs.HasKey(RetroFxEnabledKey)
                ? PlayerPrefs.GetInt(RetroFxEnabledKey)==1
                : defaultRetroFxEnabled);
            SetKeyboardSens(PlayerPrefs.GetFloat(KeyboardSensKey, defaultKeyboardSens));
            SetDpadSens(PlayerPrefs.GetFloat(DpadSensKey, defaultDpadSens));

            // Invoke events
            MusicSettingLoadedEvent.Invoke(_musicVolume);
            SoundFxSettingLoadedEvent.Invoke(_soundFxVolume);
            RetroFxLoadedEvent.Invoke(_retroEffectsEnabled);
            KeyboardSensLoadedEvent.Invoke(_keyboardSens);
            DpadSensLoadedEvent.Invoke(_dpadSens);
        }

        /// <summary>
        /// Set the Music volume
        /// </summary>
        /// <param name="newValue"></param>
        public void SetMusicVolume(float newValue)
        {
            audioMixer.SetFloat("MusicVolume", newValue);
            _musicVolume = newValue;
            MusicSettingChangedEvent.Invoke(newValue);
        }

        /// <summary>
        /// Set the SoundFxVolume
        /// </summary>
        /// <param name="newValue"></param>
        public void SetSoundFxVolume(float newValue)
        {
            audioMixer.SetFloat("SoundFxVolume", newValue);
            _soundFxVolume = newValue;
            SoundFxSettingChangedEvent.Invoke(newValue);
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
            RetroFxChangedEvent.Invoke(newValue);
        }

        /// <summary>
        /// Set the Keyboard sensitivity
        /// </summary>
        /// <param name="newValue"></param>
        public void SetKeyboardSens(float newValue)
        {
            _keyboardSens = newValue;
            KeyboardSensChangedEvent.Invoke(newValue);
        }

        /// <summary>
        /// Set the Dpad sensitivity
        /// </summary>
        /// <param name="newValue"></param>
        public void SetDpadSens(float newValue)
        {
            _dpadSens = newValue;
            DpadSensChangedEvent.Invoke(newValue);
        }
    }
}

using RetroAesthetics;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution.Menus
{
    public class SettingsManager : MonoBehaviour
    {
        [BoxGroup("Settings")] [SerializeField] private AudioMixer audioMixer;
        [BoxGroup("Settings")] [SerializeField] private Camera mainCamera;

        [BoxGroup("Defaults")] [SerializeField] private float defaultMusicVolume;
        [BoxGroup("Defaults")] [SerializeField] private float defaultSoundFxVolume;
        [BoxGroup("Defaults")] [SerializeField] private bool defaultRetroFxEnabled;
        [BoxGroup("Defaults")] [SerializeField] private float defaultKeyboardSens;
        [BoxGroup("Defaults")] [SerializeField] private float defaultDpadSens;

        [FoldoutGroup("Loaded Events")] [SerializeField] private UnityEvent<float> onMusicLoaded;
        [FoldoutGroup("Loaded Events")] [SerializeField] private UnityEvent<float> onSoundFxLoaded;
        [FoldoutGroup("Loaded Events")] [SerializeField] private UnityEvent<bool> onRetroFxLoaded;
        [FoldoutGroup("Loaded Events")] [SerializeField] private UnityEvent<float> onKeyboardSensitivityLoaded;
        [FoldoutGroup("Loaded Events")] [SerializeField] private UnityEvent<float> onDpadSensitivityLoaded;

        [FoldoutGroup("Changed Events")] [SerializeField] private UnityEvent<float> onMusicChanged;
        [FoldoutGroup("Changed Events")] [SerializeField] private UnityEvent<float> onSfxChanged;
        [FoldoutGroup("Changed Events")] [SerializeField] private UnityEvent<bool> onRetroFxChanged;
        [FoldoutGroup("Changed Events")] [SerializeField] private UnityEvent<float> onKeyboardSensitivityChanged;
        [FoldoutGroup("Changed Events")] [SerializeField] private UnityEvent<float> onDpadSensitivityChanged;

        private const string _musicVolumeKey = "MusicVolume";
        private const string _soundFxVolumeKey = "SoundsFxVolume";
        private const string _retroFxEnabledKey = "RetroFxEnabled";
        private const string _keyboardSensKey = "KeyboardSensitivity";
        private const string _dpadSensKey = "DpadSensitivity";

        private float _musicVolume;
        private float _soundFxVolume;
        private bool _retroEffectsEnabled;
        private float _keyboardSens;
        private float _dpadSens;

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
            PlayerPrefs.SetFloat(_musicVolumeKey, _musicVolume);
            PlayerPrefs.SetFloat(_soundFxVolumeKey, _soundFxVolume);
            PlayerPrefs.SetInt(_retroFxEnabledKey, _retroEffectsEnabled ? 1 : 0);
            PlayerPrefs.SetFloat(_keyboardSensKey, _keyboardSens);
            PlayerPrefs.SetFloat(_dpadSensKey, _dpadSens);
        }

        /// <summary>
        /// Load settings from User Prefs
        /// </summary>
        private void LoadSettings()
        {
            // Load settings
            SetMusicVolume(PlayerPrefs.GetFloat(_musicVolumeKey, defaultMusicVolume));
            SetSoundFxVolume(PlayerPrefs.GetFloat(_soundFxVolumeKey, defaultSoundFxVolume));
            SetRetroEffects(PlayerPrefs.HasKey(_retroFxEnabledKey)
                ? PlayerPrefs.GetInt(_retroFxEnabledKey)==1
                : defaultRetroFxEnabled);
            SetKeyboardSens(PlayerPrefs.GetFloat(_keyboardSensKey, defaultKeyboardSens));
            SetDpadSens(PlayerPrefs.GetFloat(_dpadSensKey, defaultDpadSens));

            // Invoke events
            onMusicLoaded.Invoke(_musicVolume);
            onSoundFxLoaded.Invoke(_soundFxVolume);
            onRetroFxLoaded.Invoke(_retroEffectsEnabled);
            onKeyboardSensitivityLoaded.Invoke(_keyboardSens);
            onDpadSensitivityLoaded.Invoke(_dpadSens);
        }

        /// <summary>
        /// Set the Music volume
        /// </summary>
        public void SetMusicVolume(float newValue)
        {
            audioMixer.SetFloat("MusicVolume", newValue);
            _musicVolume = newValue;
            onMusicChanged.Invoke(newValue);
        }

        /// <summary>
        /// Set the SoundFxVolume
        /// </summary>
        public void SetSoundFxVolume(float newValue)
        {
            audioMixer.SetFloat("SoundFxVolume", newValue);
            _soundFxVolume = newValue;
            onSfxChanged.Invoke(newValue);
        }

        /// <summary>
        /// Set the Retro Effects toggle
        /// </summary>
        public void SetRetroEffects(bool newValue)
        {
            RetroCameraEffect retroFx = mainCamera.GetComponent<RetroCameraEffect>();
            if (!retroFx)
            {
                return;
            }
            retroFx.enabled = newValue;
            _retroEffectsEnabled = newValue;
            onRetroFxChanged.Invoke(newValue);
        }

        /// <summary>
        /// Set the Keyboard sensitivity
        /// </summary>
        public void SetKeyboardSens(float newValue)
        {
            _keyboardSens = newValue;
            onKeyboardSensitivityChanged.Invoke(newValue);
        }

        /// <summary>
        /// Set the Dpad sensitivity
        /// </summary>
        /// <param name="newValue"></param>
        public void SetDpadSens(float newValue)
        {
            _dpadSens = newValue;
            onDpadSensitivityChanged.Invoke(newValue);
        }
    }
}
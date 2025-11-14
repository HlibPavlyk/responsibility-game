using Core.Abstractions.Menu;
using Systems.Game;
using UnityEngine;
using VContainer;

namespace Features.Menu.Options
{
    public class OptionsManager : IOptionsManager
    {
        // Constants for PlayerPrefs keys
        private const string MusicVolumeKey = "MusicVolume";
        private const string SfxVolumeKey = "SfxVolume";
        private const string DialogueSpeedKey = "DialogueSpeed";
        private const string LanguageKey = "Language";

        // Injected dependencies
        [Inject] private readonly GameSettings _gameSettings;

        // Store default values for reset functionality
        private float _defaultMusicVolume;
        private float _defaultSfxVolume;
        private float _defaultDialogueSpeed;
        private string _defaultLanguage;

        // Public properties
        public float musicVolume
        {
            get => _gameSettings.audioManagerSettings.musicVolume;
            set
            {
                _gameSettings.audioManagerSettings.musicVolume = Mathf.Clamp01(value);
                ApplyAudioSettings();
            }
        }

        public float sfxVolume
        {
            get => _gameSettings.audioManagerSettings.sfxVolume;
            set
            {
                _gameSettings.audioManagerSettings.sfxVolume = Mathf.Clamp01(value);
                ApplyAudioSettings();
            }
        }

        public float dialogueSpeed
        {
            get => 0.11f - _gameSettings.dialogueManagerSettings.typingSpeed;
            set => _gameSettings.dialogueManagerSettings.typingSpeed = 0.11f - Mathf.Clamp(value, 0.01f, 0.1f);
        }

        public string language
        {
            get => _gameSettings.generalSettings.language;
            set
            {
                _gameSettings.generalSettings.language = value;
                ApplyLanguageSettings();
            }
        }

        public void LoadSettings()
        {
            // Store defaults for reset functionality
            _defaultMusicVolume = _gameSettings.audioManagerSettings.musicVolume;
            _defaultSfxVolume = _gameSettings.audioManagerSettings.sfxVolume;
            _defaultDialogueSpeed = _gameSettings.dialogueManagerSettings.typingSpeed;
            _defaultLanguage = _gameSettings.generalSettings.language;

            // Load from PlayerPrefs or use current GameSettings values as defaults
            _gameSettings.audioManagerSettings.musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, _defaultMusicVolume);
            _gameSettings.audioManagerSettings.sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, _defaultSfxVolume);
            _gameSettings.dialogueManagerSettings.typingSpeed = PlayerPrefs.GetFloat(DialogueSpeedKey, _defaultDialogueSpeed);
            _gameSettings.generalSettings.language = PlayerPrefs.GetString(LanguageKey, _defaultLanguage);

            ApplyAllSettings();
        }

        public void SaveSettings()
        {
            PlayerPrefs.SetFloat(MusicVolumeKey, _gameSettings.audioManagerSettings.musicVolume);
            PlayerPrefs.SetFloat(SfxVolumeKey, _gameSettings.audioManagerSettings.sfxVolume);
            PlayerPrefs.SetFloat(DialogueSpeedKey, _gameSettings.dialogueManagerSettings.typingSpeed);
            PlayerPrefs.SetString(LanguageKey, _gameSettings.generalSettings.language);
            PlayerPrefs.Save();

            Debug.Log("Settings saved successfully");
        }

        public void ResetToDefaults()
        {
            _gameSettings.audioManagerSettings.musicVolume = _defaultMusicVolume;
            _gameSettings.audioManagerSettings.sfxVolume = _defaultSfxVolume;
            _gameSettings.dialogueManagerSettings.typingSpeed = _defaultDialogueSpeed;
            _gameSettings.generalSettings.language = _defaultLanguage;

            ApplyAllSettings();
            Debug.Log("Settings reset to defaults");
        }

        private void ApplyAllSettings()
        {
            ApplyAudioSettings();
            ApplyLanguageSettings();
        }

        private void ApplyAudioSettings()
        {
            // Audio volumes will be used by your audio system
            // You can extend this to use an Audio Mixer if needed
            Debug.Log($"Audio settings applied - Music: {_gameSettings.audioManagerSettings.musicVolume}, " +
                      $"SFX: {_gameSettings.audioManagerSettings.sfxVolume}");
        }

        private void ApplyLanguageSettings()
        {
            // Language change logic - you can implement localization here
            Debug.Log($"Language applied: {_gameSettings.generalSettings.language}");
        }
    }
}
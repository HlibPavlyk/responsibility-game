using System;
using Core.Abstractions;
using Core.Abstractions.Menu;
using Core.DI;
using Features.Menu.PauseMenu;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VContainer;

namespace Features.Menu.Options
{
    [InjectableMonoBehaviour]
    public class OptionsWindowUI : MonoBehaviour
    {
        [Header("Panel References")]
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private MonoBehaviour menuUI;

        [Header("Audio Settings")]
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private TextMeshProUGUI musicVolumeText;
        [SerializeField] private TextMeshProUGUI sfxVolumeText;

        [Header("Dialogue Settings")]
        [SerializeField] private Slider dialogueSpeedSlider;
        [SerializeField] private TextMeshProUGUI dialogueSpeedText;

        [Header("Language Settings")]
        [SerializeField] private TMP_Dropdown languageDropdown;

        [Header("Buttons")]
        [SerializeField] private Button resetButton;
        [SerializeField] private Button backButton;

        [Header("Animation (Optional)")]
        [SerializeField] private PauseMenuAnimator menuAnimator;

        // Injected dependencies
        [Inject] private readonly IOptionsManager _optionsManager;
        [Inject] private readonly IInputManager _inputManager;

        // Unity components
        private EventSystem _eventSystem;
        private Controls _controls;
        private Action<InputAction.CallbackContext> _onPause;
        private IMenuInteraction _menuInteraction;

        private void Awake()
        {
            _eventSystem = EventSystem.current;
            _controls = new Controls();

            // Cache the IMenuInteraction interface
            _menuInteraction = menuUI as IMenuInteraction;

            // Hide panel by default
            if (optionsPanel != null)
            {
                optionsPanel.SetActive(false);
            }
        }

        private void Start()
        {
            LoadCurrentSettings();
        }

        private void OnEnable()
        {
            // Audio sliders
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);

            // Dialogue settings
            dialogueSpeedSlider.onValueChanged.AddListener(OnDialogueSpeedChanged);

            // Language settings
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

            // Buttons
            resetButton.onClick.AddListener(OnResetClicked);
            backButton.onClick.AddListener(OnBackClicked);
            
            _controls.Enable();
            _onPause = context => _inputManager?.PauseButtonPressed(context);

            // connect pause button
            _controls.Player.Pause.started += _onPause;
            _controls.Player.Pause.performed += _onPause;
            _controls.Player.Pause.canceled += _onPause;
        }

        private void OnDisable()
        {
            // Clean up listeners
            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
            if (dialogueSpeedSlider != null)
                dialogueSpeedSlider.onValueChanged.RemoveListener(OnDialogueSpeedChanged);
            if (languageDropdown != null)
                languageDropdown.onValueChanged.RemoveListener(OnLanguageChanged);
            if (resetButton != null)
                resetButton.onClick.RemoveListener(OnResetClicked);
            if (backButton != null)
                backButton.onClick.RemoveListener(OnBackClicked);
            
            _controls.Disable();
        }

        private void Update()
        {
            // Handle ESC key to close options
            if (_inputManager.GetPausePressed())
            {
                OnBackClicked();
            }
        }

        private void LoadCurrentSettings()
        {
            // Load settings without triggering events
            musicVolumeSlider.SetValueWithoutNotify(_optionsManager.musicVolume);
            sfxVolumeSlider.SetValueWithoutNotify(_optionsManager.sfxVolume);
            dialogueSpeedSlider.SetValueWithoutNotify(_optionsManager.dialogueSpeed);

            // Set language dropdown
            SetDropdownLanguage();

            UpdateVolumeTexts();
            UpdateDialogueSpeedText();
        }

        public void Show()
        {
            if (menuAnimator != null)
            {
                menuAnimator.Show();
            }
            else
            {
                optionsPanel.SetActive(true);
            }

            LoadCurrentSettings();
            SelectFirstButton();
        }

        private void Hide()
        {
            // Save settings when closing options
            _optionsManager.SaveSettings();
            Debug.Log("Settings saved on close!");

            if (menuAnimator)
            {
                menuAnimator.Hide();
            }
            else
            {
                optionsPanel.SetActive(false);
            }

            if (_eventSystem)
            {
                _eventSystem.SetSelectedGameObject(null);
            }

            if (menuUI != null)
            {
                menuUI.gameObject.SetActive(true);

                // Restore interaction with parent menu using interface
                if (_menuInteraction != null)
                {
                    _menuInteraction.EnableInteraction(true);
                    _menuInteraction.SelectOptionsButton();
                }
            }
        }

		private void SelectFirstButton()
        {
            if (_eventSystem != null && musicVolumeSlider != null)
            {
                _eventSystem.SetSelectedGameObject(musicVolumeSlider.gameObject);
            }
        }

        private void OnMusicVolumeChanged(float value)
        {
            _optionsManager.musicVolume = value;
            UpdateVolumeTexts();
        }

        private void OnSfxVolumeChanged(float value)
        {
            _optionsManager.sfxVolume = value;
            UpdateVolumeTexts();
        }

        private void OnDialogueSpeedChanged(float value)
        {
            _optionsManager.dialogueSpeed = value;
            UpdateDialogueSpeedText();
        }

        private void OnLanguageChanged(int index)
        {
            // 0 = English
            _optionsManager.language = index switch
            {
                0 => "en",
                _ => _optionsManager.language
            };
        }

        private void SetDropdownLanguage()
        {
            var languageIndex = _optionsManager.language switch
            {
                _ => 0
            };
            
            languageDropdown.SetValueWithoutNotify(languageIndex);
        }
        
        private void UpdateVolumeTexts()
        {
            if (musicVolumeText != null)
                musicVolumeText.text = Mathf.RoundToInt(_optionsManager.musicVolume * 100) + "%";

            if (sfxVolumeText != null)
                sfxVolumeText.text = Mathf.RoundToInt(_optionsManager.sfxVolume * 100) + "%";
        }

        private void UpdateDialogueSpeedText()
        {
            // 0.01 / 0.04 / 0.1
            if (dialogueSpeedText != null)
            {
                // Convert speed to a more user-friendly scale (slower = higher number)
                dialogueSpeedText.text = Mathf.RoundToInt(_optionsManager.dialogueSpeed * 1000) + "%";
            }
        }

        private void OnResetClicked()
        {
            _optionsManager.ResetToDefaults();
            LoadCurrentSettings();
        }

        private void OnBackClicked() => Hide();
    }
}
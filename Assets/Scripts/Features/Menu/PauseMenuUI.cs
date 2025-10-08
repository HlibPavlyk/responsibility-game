using System;
using Core.Abstractions;
using Core.DI;
using Core.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VContainer;

namespace Features.Menu
{
    [InjectableMonoBehaviour]
    public class PauseMenuUI : MonoBehaviour
    {
        // serialized fields
        [Header("UI References")]
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button quitButton;
        
        [Header("Animation (Optional)")]
        [SerializeField] private PauseMenuAnimator menuAnimator;

        // injected dependencies
        [Inject] private readonly IPauseMenuManager _pauseMenuManager;
        [Inject] private readonly IInputManager _inputManager;
        
        // unity components
        private Controls _controls;
        private EventSystem _eventSystem;
        
        // actions for input controls
        private Action<InputAction.CallbackContext> _onPause;

        private void Awake()
        {
            pauseMenuPanel.SetActive(false);
            _controls = new Controls();
            _eventSystem = EventSystem.current;
        }

        private void OnEnable()
        {
            // set game event listeners
            GameEvents.Game.OnGamePaused += ShowMenu;
            GameEvents.Game.OnGameResumed += HideMenu;
            
            // set button listeners
            resumeButton.onClick.AddListener(OnResumeClicked);
            optionsButton.onClick.AddListener(OnOptionsClicked);
            resetButton.onClick.AddListener(OnResetClicked);
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            quitButton.onClick.AddListener(OnQuitClicked);
            
            _controls.Enable();
            _onPause = context => _inputManager?.PauseButtonPressed(context);

            // connect pause button
            _controls.Player.Pause.started += _onPause;
            _controls.Player.Pause.performed += _onPause;
            _controls.Player.Pause.canceled += _onPause;
        }

        private void OnDisable()
        {
            GameEvents.Game.OnGamePaused -= ShowMenu;
            GameEvents.Game.OnGameResumed -= HideMenu;
            
            resumeButton.onClick.RemoveListener(OnResumeClicked);
            optionsButton.onClick.RemoveListener(OnOptionsClicked);
            resetButton.onClick.RemoveListener(OnResetClicked);
            mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
            quitButton.onClick.RemoveListener(OnQuitClicked);
            
            _controls.Disable();
            
            // disconnect pause button
            _controls.Player.Pause.started -= _onPause;
            _controls.Player.Pause.performed -= _onPause;
            _controls.Player.Pause.canceled -= _onPause;
        }

        private void Update()
        {
            if (_inputManager.GetPausePressed())
            {
                Debug.Log("Pause button pressed");
                _pauseMenuManager.TogglePause();
            }
        }

        private void ShowMenu()
        {
            if (menuAnimator != null)
            {
                menuAnimator.Show();
            }
            else
            {
                pauseMenuPanel.SetActive(true);
            }
            
            SelectFirstButton();
        }

        private void HideMenu()
        {
            if (menuAnimator != null)
            {
                menuAnimator.Hide();
            }
            else
            {
                pauseMenuPanel.SetActive(false);
            }
            
            if (_eventSystem != null)
            {
                _eventSystem.SetSelectedGameObject(null);
            }
        }
        
        private void SelectFirstButton()
        {
            if (_eventSystem != null && resumeButton != null)
            {
                _eventSystem.SetSelectedGameObject(resumeButton.gameObject);
            }
        }

        private void OnResumeClicked() => _pauseMenuManager.Resume();
        private void OnOptionsClicked() => _pauseMenuManager.OpenOptions();
        private void OnResetClicked() => _pauseMenuManager.ResetLevel();
        private void OnMainMenuClicked() => _pauseMenuManager.ReturnToMainMenu();
        private void OnQuitClicked() => _pauseMenuManager.QuitGame();
    }
}

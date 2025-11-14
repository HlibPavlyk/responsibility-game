using System;
using Core.Abstractions;
using Core.Abstractions.Menu;
using Core.DI;
using Core.Events;
using Features.Menu.Options;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VContainer;

namespace Features.Menu.PauseMenu
{
    [InjectableMonoBehaviour]
    public class PauseMenuUI : MonoBehaviour, IMenuInteraction
    {
        // serialized fields
        [Header("UI References")]
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private OptionsWindowUI optionsWindowUI;
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

        // IMenuInteraction implementation
        public void EnableInteraction(bool enable)
        {
            if (resumeButton) resumeButton.interactable = enable;
            if (optionsButton) optionsButton.interactable = enable;
            if (resetButton) resetButton.interactable = enable;
            if (mainMenuButton) mainMenuButton.interactable = enable;
            if (quitButton) quitButton.interactable = enable;
        }
        
        public void SelectOptionsButton()
        {
            if (_eventSystem && optionsButton)
            {
                _eventSystem.SetSelectedGameObject(optionsButton.gameObject);
            }
        }
        
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
            // Don't handle pause input when options menu is open
            if (optionsWindowUI && optionsWindowUI.gameObject.activeSelf)
            {
                return;
            }

            if (_inputManager.GetPausePressed())
            {
                Debug.Log("Pause button pressed");
                _pauseMenuManager.TogglePause();
            }
        }

        private void ShowMenu()
        {
            if (menuAnimator)
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
            if (menuAnimator)
            {
                menuAnimator.Hide();
            }
            else
            {
                pauseMenuPanel.SetActive(false);
            }

            if (_eventSystem)
            {
                _eventSystem.SetSelectedGameObject(null);
            }
        }
        
        private void SelectFirstButton()
        {
            if (_eventSystem && resumeButton)
            {
                _eventSystem.SetSelectedGameObject(resumeButton.gameObject);
            }
        }

        private void OnResumeClicked() => _pauseMenuManager.Resume();
        private void OnOptionsClicked()
        {
            // Disable pause menu interaction while options is open
            EnableInteraction(false);
            _pauseMenuManager.OpenOptions(optionsWindowUI);
        }

        private void OnResetClicked() => _pauseMenuManager.ResetLevel();
        private void OnMainMenuClicked() => _pauseMenuManager.ReturnToMainMenu();
        private void OnQuitClicked() => _pauseMenuManager.QuitGame();
    }
}

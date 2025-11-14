using System;
using Core.Abstractions;
using Core.DI;
using Features.Menu.MainMenu;
using Features.Menu.PauseMenu;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using VContainer;

namespace Features.Menu.About
{
    [InjectableMonoBehaviour]
    public class AboutWindowUI : MonoBehaviour
    {
        [Header("Panel References")]
        [SerializeField] private GameObject aboutPanel;
        [SerializeField] private MenuManagerUI menuUI;

        [Header("Buttons")]
        [SerializeField] private Button backButton;

        [Header("Animation (Optional)")]
        [SerializeField] private PauseMenuAnimator menuAnimator;

        // Injected dependencies
        [Inject] private readonly IInputManager _inputManager;

        // Unity components
        private EventSystem _eventSystem;
        private Controls _controls;
        private Action<InputAction.CallbackContext> _onPause;

        private void Awake()
        {
            _eventSystem = EventSystem.current;
            _controls = new Controls();

            // Hide panel by default
            if (aboutPanel)
            {
                aboutPanel.SetActive(false);
            }
        }

        private void OnEnable()
        {
            // Buttons
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
            // Buttons
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

        public void Show()
        {
            if (menuAnimator)
            {
                menuAnimator.Show();
            }
            else
            {
                aboutPanel.SetActive(true);
            }

            SelectFirstButton();
        }

        private void Hide()
        {
            if (menuAnimator)
            {
                menuAnimator.Hide();
            }
            else
            {
                aboutPanel.SetActive(false);
            }

            if (_eventSystem)
            {
                _eventSystem.SetSelectedGameObject(null);
            }

            if (menuUI)
            {
                menuUI.gameObject.SetActive(true);

                // Restore interaction with parent menu using interface
                menuUI.EnableInteraction(true);
                menuUI.SelectAboutButton();
            }
        }

		private void SelectFirstButton()
        {
            if (_eventSystem != null && backButton != null)
            {
                _eventSystem.SetSelectedGameObject(backButton.gameObject);
            }
        }

        private void OnBackClicked() => Hide();
    }
}
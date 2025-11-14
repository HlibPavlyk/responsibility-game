using Core.Abstractions;
using Core.Abstractions.Menu;
using Core.DI;
using Features.Menu.About;
using Features.Menu.Options;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace Features.Menu.MainMenu
{
    [InjectableMonoBehaviour]
    public class MenuManagerUI : MonoBehaviour, IMenuInteraction
    {
        // serialized fields
        [Header("UI References")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private OptionsWindowUI optionsWindowUI;
        [SerializeField] private AboutWindowUI aboutWindowUI;
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button aboutButton;
        [SerializeField] private Button quitButton;

        // injectable dependencies
        [Inject] private readonly IMenuManager _menuManager;
        [Inject] private readonly IInputManager _inputManager;

        // unity components
        private EventSystem _eventSystem;
        
        // IMenuInteraction implementation
        public void EnableInteraction(bool enable)
        {
            if (newGameButton) newGameButton.interactable = enable;
            if (continueButton) continueButton.interactable = enable;
            if (settingsButton) settingsButton.interactable = enable;
            if (aboutButton) aboutButton.interactable = enable;
            if (quitButton) quitButton.interactable = enable;
        }

        public void SelectOptionsButton()
        {
            if (_eventSystem && settingsButton)
            {
                _eventSystem.SetSelectedGameObject(settingsButton.gameObject);
            }
        }
        
        public void SelectAboutButton()
        {
            if (_eventSystem && aboutButton)
            {
                _eventSystem.SetSelectedGameObject(aboutButton.gameObject);
            }
        }

        private void Awake()
        {
            _eventSystem = EventSystem.current;
        }

        private void Start()
        {
            newGameButton.onClick.AddListener(OnNewGame);
            continueButton.onClick.AddListener(OnContinueGame);
            settingsButton.onClick.AddListener(OnOpenSettings);
            aboutButton.onClick.AddListener(OnShowAboutInfo);
            quitButton.onClick.AddListener(OnExitGame);

            // Show the main menu
            mainMenuPanel.SetActive(true);

            // Disable continue button if no save exists
            continueButton.interactable = _menuManager.hasSaveGame;

            // select first button and setup button navigation
            SetupButtonNavigation();
            SelectFirstButton();
        }

        private void OnNewGame()
        {
            _menuManager.NewGame();
        }

        private void OnContinueGame()
        {
            if (_menuManager.hasSaveGame)
            {
                _menuManager.ContinueGame();
            }
        }

        private void OnOpenSettings()
        {
            EnableInteraction(false);
            _menuManager.OpenSettings(optionsWindowUI);
        }

        private void OnShowAboutInfo()
        {
            EnableInteraction(false);
            _menuManager.ShowAboutInfo(aboutWindowUI);
        }

        private void OnExitGame()
        {
            _menuManager.ExitGame();
        }

        private void SelectFirstButton()
        {
            if (_eventSystem != null && newGameButton != null)
            {
                _eventSystem.SetSelectedGameObject(newGameButton.gameObject);
            }
        }

        private void SetupButtonNavigation()
        {
            // Set up navigation for New Game button
            var newGameNav = newGameButton.navigation;
            newGameNav.selectOnDown = _menuManager.hasSaveGame ? continueButton : settingsButton;
            newGameButton.navigation = newGameNav;

            // Set up navigation for Settings button
            var settingsNav = settingsButton.navigation;
            settingsNav.selectOnUp = _menuManager.hasSaveGame ? continueButton : newGameButton;
            settingsButton.navigation = settingsNav;

            // If Continue button is active, set its navigation
            if (_menuManager.hasSaveGame)
            {
                var continueNav = continueButton.navigation;
                continueNav.selectOnUp = newGameButton;
                continueNav.selectOnDown = settingsButton;
                continueButton.navigation = continueNav;
            }
        }

        private void OnDestroy()
        {
            // Clean up button listeners
            newGameButton.onClick.RemoveListener(OnNewGame);
            continueButton.onClick.RemoveListener(OnContinueGame);
            settingsButton.onClick.RemoveListener(OnOpenSettings);
            aboutButton.onClick.RemoveListener(OnShowAboutInfo);
            quitButton.onClick.RemoveListener(OnExitGame);
        }
    }
}
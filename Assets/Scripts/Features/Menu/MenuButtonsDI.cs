using Core.Abstractions;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace ResponsibilityGame.GameSystems.Menu
{
    public class MenuButtonsDI : MonoBehaviour
    {
        [Header("Menu Buttons")]
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;
        
        // DI залежності
        [Inject] private IMenuManager menuManager;
        [Inject] private ISaveLoadManager saveLoadManager;

        private void Start()
        {
            SetupButtons();
            UpdateContinueButtonState();
        }

        private void SetupButtons()
        {
            // Налаштовуємо обробники подій для кнопок
            if (newGameButton != null)
                newGameButton.onClick.AddListener(OnNewGameClick);
                
            if (continueButton != null)
                continueButton.onClick.AddListener(OnContinueClick);
                
            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettingsClick);
                
            if (exitButton != null)
                exitButton.onClick.AddListener(OnExitClick);
        }

        private void UpdateContinueButtonState()
        {
            // Активуємо кнопку Continue тільки якщо є збережена гра
            if (continueButton != null)
            {
                continueButton.interactable = saveLoadManager.HasSaveFile();
            }
        }

        private void OnNewGameClick()
        {
            Debug.Log("New Game clicked");
            menuManager.NewGame();
        }

        private void OnContinueClick()
        {
            Debug.Log("Continue clicked");
            menuManager.ContinueGame();
        }

        private void OnSettingsClick()
        {
            Debug.Log("Settings clicked");
            menuManager.OpenSettings();
        }

        private void OnExitClick()
        {
            Debug.Log("Exit clicked");
            menuManager.ExitGame();
        }

        private void OnDestroy()
        {
            // Очищуємо обробники подій
            if (newGameButton != null)
                newGameButton.onClick.RemoveListener(OnNewGameClick);
                
            if (continueButton != null)
                continueButton.onClick.RemoveListener(OnContinueClick);
                
            if (settingsButton != null)
                settingsButton.onClick.RemoveListener(OnSettingsClick);
                
            if (exitButton != null)
                exitButton.onClick.RemoveListener(OnExitClick);
        }
    }
}

using Core.DI;
using Core.Events;
using Core.Interfaces;
using ResponsibilityGame.Core.DI;
using UnityEngine;
using VContainer;
using ResponsibilityGame.Core.Interfaces;

namespace ResponsibilityGame.GameSystems.Menu
{
    public class MenuManagerService : IMenuManager
    {
        /*private readonly string startSceneName;
        private readonly ISaveLoadManager saveLoadManager;
        private readonly IPlayerManager playerManager;
        
        private GameObject settingsPanel;
        private GameObject background;
        private GameState gameState;

        [Inject]
        public MenuManagerService(
            string startSceneName,
            ISaveLoadManager saveLoadManager,
            IPlayerManager playerManager,
            GameState gameState)
        {
            this.startSceneName = startSceneName;
            this.saveLoadManager = saveLoadManager;
            this.playerManager = playerManager;
            this.gameState = gameState;
        }*/

        /*public void Initialize(string sceneName, GameObject canvas)
        {
            if (canvas != null)
            {
                background = canvas.transform.Find("Background")?.gameObject;
                if (background != null)
                {
                    settingsPanel = background.transform.Find("SettingsWindow")?.gameObject;
                    if (settingsPanel != null)
                    {
                        settingsPanel.SetActive(false);
                    }
                }
            }
            
            Debug.Log("MenuManagerService initialized successfully");
        }*/
        
        [Inject] private GameState gameState;
        [Inject] private MenuManagerSettings settings;
        [Inject] private ISaveLoadManager saveLoadManager;

        public void NewGame()
        {
            saveLoadManager.DeleteSaves();
            
            GameEvents.Level.levelExit.Invoke(settings.StartSceneName, "");
            
            gameState.playerStats.currentSceneName = settings.StartSceneName;
            saveLoadManager.SaveGame();
        }

        public void ContinueGame()
        {
            saveLoadManager.LoadGame();
            GameEvents.Level.levelExit.Invoke(gameState.playerStats.currentSceneName, "");
        }

        public void LoadGame()
        {
            // Implementation for load game functionality
        }

        public void OpenSettings()
        {
            /*if (settingsPanel != null)
            {
                bool isActive = settingsPanel.activeSelf;
                settingsPanel.SetActive(!isActive);
            }*/
        }

        public void ShowAboutInfo()
        {
            // Implementation for about info functionality
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}

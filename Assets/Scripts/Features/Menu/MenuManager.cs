using Core.Abstractions;
using Core.DI;
using Core.Events;
using UnityEngine;
using VContainer;
using Systems.Game;

namespace ResponsibilityGame.GameSystems.Menu
{
    public class MenuManager : IMenuManager
    {
        [Inject] private GameState gameState;
        [Inject] private MenuManagerSettings settings;
        [Inject] private ISaveLoadManager saveLoadManager;

        public void NewGame()
        {
            saveLoadManager.DeleteSaves();
            
            GameEvents.Level.LevelExit.Invoke(settings.startSceneName, "");
            
            gameState.playerStats.currentSceneName = settings.startSceneName;
            saveLoadManager.SaveGame();
        }

        public void ContinueGame()
        {
            saveLoadManager.LoadGame();
            GameEvents.Level.LevelExit.Invoke(gameState.playerStats.currentSceneName, "");
        }

        public void LoadGame()
        {
            // Implementation for load game functionality
        }

        public void OpenSettings()
        {
            // Implementation for OpenSettings functionality
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

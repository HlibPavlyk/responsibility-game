using Core.Abstractions;
using Core.Abstractions.Menu;
using Core.Events;
using Features.Menu.Options;
using Systems.Game;
using UnityEngine;
using VContainer;

namespace Features.Menu.PauseMenu
{
    public class PauseMenuManager : IPauseMenuManager
    {
        // injected dependencies
        [Inject] private readonly IMenuManager _menuManager;
        [Inject] private readonly ISaveLoadManager _saveLoadManager;
        [Inject] private readonly GameState _gameState;

        public void TogglePause()
        {
            if (_gameState.isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        private void Pause()
        {
            _gameState.isPaused = true;
            Time.timeScale = 0f;
            GameEvents.Game.OnGamePaused?.Invoke();
        }

        public void Resume()
        {
            _gameState.isPaused = false;
            Time.timeScale = 1f;
            GameEvents.Game.OnGameResumed?.Invoke();
        }

        public void OpenOptions(OptionsWindowUI optionsWindowUI)
        {
            if (optionsWindowUI)
            {
                Debug.LogWarning("Options window UI is null");
            }
            
            optionsWindowUI.Show();
        }

        public void ResetLevel()
        {
            Resume();
            var currentScene = _gameState.playerStats.currentSceneName;
            GameEvents.Level.LevelExit?.Invoke(currentScene, "");
        }

        public void ReturnToMainMenu()
        {
            Resume();
            _saveLoadManager.SaveGame();
            GameEvents.Level.LevelExit?.Invoke("MainMenu", "");
        }

        public void QuitGame()
        {
            Resume();
            _menuManager.ExitGame();
        }
    }
}

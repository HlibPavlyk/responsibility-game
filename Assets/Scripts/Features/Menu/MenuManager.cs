using System.Runtime.InteropServices;
using Core.Abstractions;
using Core.Events;
using UnityEngine;
using VContainer;
using Systems.Game;

namespace Features.Menu
{
    public class MenuManager : IMenuManager
    {
        // injectable dependencies
        [Inject] private readonly GameState _gameState;
        [Inject] private readonly MenuManagerSettings _settings;
        [Inject] private readonly ISaveLoadManager _saveLoadManager;

        // public properties
        public bool hasSaveGame => _saveLoadManager.HasSaveFile();
        
        // import reload function from jslib
        [DllImport("__Internal")] private static extern void ReloadPage();

        public void NewGame()
        {
            _saveLoadManager.DeleteSaves();
            _gameState.playerStats.currentSceneName = _settings.startSceneName;
            GameEvents.Level.LevelExit?.Invoke(_settings.startSceneName, "");
        }

        public void ContinueGame()
        {
            if (_saveLoadManager.HasSaveFile())
            {
                _saveLoadManager.LoadGame();
                GameEvents.Level.LevelExit?.Invoke(_gameState.playerStats.currentSceneName, "");
            }
        }

        public void LoadGame()
        {
            // Will be implemented to load a specific save
            Debug.Log("Loading game...");
        }

        public void OpenSettings()
        {
            // Will be implemented to show settings panel
            Debug.Log("Opening settings...");
        }

        public void ShowAboutInfo()
        {
            // Will be implemented to show about panel
            Debug.Log("Showing about info...");
        }

        public void ExitGame()
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
                ReloadPage();
            #elif UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}

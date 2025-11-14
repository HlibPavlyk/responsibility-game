using System.Runtime.InteropServices;
using Core.Abstractions;
using Core.Abstractions.Menu;
using Core.Events;
using Features.Menu.About;
using Features.Menu.Options;
using Systems.Game;
using UnityEngine;
using VContainer;

namespace Features.Menu.MainMenu
{
    public class MenuManager : IMenuManager
    {
        // injectable dependencies
        [Inject] private readonly GameState _gameState;
        [Inject] private readonly MenuManagerSettings _settings;
        [Inject] private readonly ISaveLoadManager _saveLoadManager;
        [Inject] private readonly IOptionsManager _optionsManager;

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

        public void OpenSettings(OptionsWindowUI optionsWindowUI)
        {
            if (!optionsWindowUI)
            {
                Debug.LogWarning("Options window UI is null");
            }
            
            optionsWindowUI.Show();
        }

        public void ShowAboutInfo(AboutWindowUI aboutWindowUI)
        {
            if (!aboutWindowUI)
            {
                Debug.LogWarning("About window UI is null");
            }
            
            aboutWindowUI.Show();
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

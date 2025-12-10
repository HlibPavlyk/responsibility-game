using Core.Abstractions;
using Systems.Game;
using UnityEngine;
using VContainer;

namespace Systems.SaveLoad
{
    public class WebSaveLoadManager : ISaveLoadManager
    {
        private const string WebSaveKey = "GameSave";

        [Inject] private readonly GameState _gameState;
        [Inject] private readonly StoryState _storyState;

        public void SaveGame()
        {
            var saveData = new SaveData
            {
                currentSceneName = _gameState.playerStats.currentSceneName,
                activeFlags = _storyState.GetAllFlags(),
                firedTriggers = _storyState.GetAllTriggeredIDs()
            };

            var json = JsonUtility.ToJson(saveData, true);
            PlayerPrefs.SetString(WebSaveKey, json);
            PlayerPrefs.Save();
            Debug.Log("Game saved in browser PlayerPrefs");
        }

        public void LoadGame()
        {
            if (!PlayerPrefs.HasKey(WebSaveKey))
            {
                Debug.LogError("No save found in browser PlayerPrefs");
                return;
            }

            var json = PlayerPrefs.GetString(WebSaveKey);
            var saveData = JsonUtility.FromJson<SaveData>(json);

            _gameState.playerStats.currentSceneName = saveData.currentSceneName;
            _storyState.LoadFlags(saveData.activeFlags);
            _storyState.LoadTriggers(saveData.firedTriggers);

            Debug.Log("Game loaded from browser PlayerPrefs");
        }

        public void DeleteSaves()
        {
            if (PlayerPrefs.HasKey(WebSaveKey))
            {
                PlayerPrefs.DeleteKey(WebSaveKey);
                Debug.Log("Browser save deleted");
            }
        }

        public bool HasSaveFile()
        {
            return PlayerPrefs.HasKey(WebSaveKey);
        }
    }
}
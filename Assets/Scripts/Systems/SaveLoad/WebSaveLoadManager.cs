using ResponsibilityGame.Core.Interfaces;
using Systems.Game;
using UnityEngine;
using VContainer;

namespace Systems.SaveLoad
{
    public class WebSaveLoadManager : ISaveLoadManager
    {
        private const string WebSaveKey = "GameSave";

        [Inject] private readonly GameState _gameState;

        public void SaveGame()
        {
            var json = JsonUtility.ToJson(_gameState.playerStats, true);
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
            JsonUtility.FromJsonOverwrite(json, _gameState.playerStats);
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
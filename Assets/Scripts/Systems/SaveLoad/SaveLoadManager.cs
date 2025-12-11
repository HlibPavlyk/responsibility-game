using System.IO;
using Core.Abstractions;
using Systems.Game;
using UnityEngine;
using VContainer;

namespace Systems.SaveLoad
{
    public class SaveLoadManager : ISaveLoadManager
    {
        private string saveDirectory => Path.Combine(Application.persistentDataPath, "Saves");
        private string saveFile => Path.Combine(saveDirectory, "LastSave.json");

        [Inject] private readonly GameState _gameState;
        [Inject] private readonly StoryState _storyState;

        public void SaveGame()
        {
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            var saveData = new SaveData
            {
                currentSceneName = _gameState.playerStats.currentSceneName,
                activeFlags = _storyState.GetAllFlags(),
                firedTriggers = _storyState.GetAllTriggeredIDs()
            };

            var json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(saveFile, json);

            Debug.Log($"Game saved to: {saveFile}");
        }

        public void LoadGame()
        {
            if (!File.Exists(saveFile))
            {
                Debug.LogError($"Save file {saveFile} does not exist in {saveDirectory}");
                return;
            }

            var json = File.ReadAllText(saveFile);
            var saveData = JsonUtility.FromJson<SaveData>(json);

            _gameState.playerStats.currentSceneName = saveData.currentSceneName;
            _storyState.LoadFlags(saveData.activeFlags);
            _storyState.LoadTriggers(saveData.firedTriggers);

            Debug.Log($"Game loaded from: {saveFile}");
        }

        public void DeleteSaves()
        {
            if (HasSaveFile())
            {
                File.Delete(saveFile);
                Debug.Log("Save file deleted");
            }
        }

        public bool HasSaveFile()
        {
            return File.Exists(saveFile);
        }

        private bool IsExist(string filePath)
        {
            return File.Exists(filePath) || Directory.Exists(filePath);
        }
    }
}

using System.IO;
using ResponsibilityGame.Core.Interfaces;
using UnityEngine;
using VContainer;

namespace GameSystems.SaveLoad
{
    public class SaveLoadManagerService : ISaveLoadManager
    {
        private string saveDirectory => Path.Combine(Application.persistentDataPath, "Saves");
        private string saveFile => Path.Combine(saveDirectory, "LastSave.json");

        [Inject] private GameState gameState;
        
        public void SaveGame()
        {
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }
            
            var json = JsonUtility.ToJson(gameState.playerStats, true);
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
            JsonUtility.FromJsonOverwrite(json, gameState.playerStats);
            
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

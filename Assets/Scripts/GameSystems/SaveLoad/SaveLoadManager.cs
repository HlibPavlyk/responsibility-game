using System.IO;
using UnityEngine;

public static class SaveLoadManager
{
    private static readonly string saveDirectory;
    private static readonly string saveFile;

    static SaveLoadManager()
    {
        saveDirectory = Path.Combine(Application.persistentDataPath, "Saves");
        saveFile = Path.Combine(saveDirectory, "LastSave.json");
    }

    private static bool IsExist(string filePath)
    {
        return File.Exists(filePath) || Directory.Exists(filePath);
    }

    public static void SaveGame()
    {
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        var json = JsonUtility.ToJson(GameManager.Instance.PlayerManager.PlayerStats, true);
        File.WriteAllText(saveFile, json);
    }

    public static void LoadGame()
    {
        if (!File.Exists(saveFile))
        {
            Debug.LogError($"Save file {saveFile} does not exist in {saveDirectory}");
            return;
        }
        
        var json = File.ReadAllText(saveFile);
        JsonUtility.FromJsonOverwrite(json, GameManager.Instance.PlayerManager.PlayerStats);
    }

    public static void DeleteSaves()
    {
        if (IsExist(saveFile))
        {
            File.Delete(saveFile);
        }
    }
}

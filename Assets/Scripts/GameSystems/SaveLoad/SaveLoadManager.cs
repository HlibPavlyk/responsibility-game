using System.IO;
using UnityEngine;

public static class SaveLoadManager
{
    private static readonly string SaveDirectory;
    private static readonly string SaveFile;

    static SaveLoadManager()
    {
        SaveDirectory = Path.Combine(Application.persistentDataPath, "Saves");
        SaveFile = Path.Combine(SaveDirectory, "LastSave.json");
    }

    private static bool IsExist(string filePath)
    {
        return File.Exists(filePath) || Directory.Exists(filePath);
    }

    public static void SaveGame()
    {
        if (!Directory.Exists(SaveDirectory))
        {
            Directory.CreateDirectory(SaveDirectory);
        }

        var json = JsonUtility.ToJson(GameManager.Instance.PlayerManager.PlayerStats, true);
        File.WriteAllText(SaveFile, json);
    }

    public static void LoadGame()
    {
        if (!File.Exists(SaveFile))
        {
            Debug.LogError($"Save file {SaveFile} does not exist in {SaveDirectory}");
            return;
        }
        
        var json = File.ReadAllText(SaveFile);
        JsonUtility.FromJsonOverwrite(json, GameManager.Instance.PlayerManager.PlayerStats);
    }

    public static void DeleteSaves()
    {
        if (IsExist(SaveFile))
        {
            File.Delete(SaveFile);
        }
    }
}

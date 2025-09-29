using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Core.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public static class SaveLoadManager
{
    private static readonly string saveDirectory;
    private static readonly string saveFile;

    //todo update that
    /*[Inject] private IPlayerManager playerManager;*/
    
    static SaveLoadManager()
    {
        saveDirectory = Application.persistentDataPath + "/Saves";
        saveFile = saveDirectory + "/LastSave.txt";
    }

    private static bool IsExist(string filePath)
    {
        return File.Exists(filePath) || Directory.Exists(filePath);
    }

    public static void SaveGame()
    {
        if (!IsExist(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(saveFile);

        string json = JsonUtility.ToJson(string.Empty /*playerManager.PlayerStats*/);
        bf.Serialize(file, json);
        file.Close();
    }

    public static void LoadGame()
    {
        if (!IsExist(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        BinaryFormatter bf = new BinaryFormatter();

        if (IsExist(saveFile))
        {
            FileStream file = File.Open(saveFile, FileMode.Open);
            JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), string.Empty /*GameManager.Instance.PlayerManager.PlayerStats*/);
            file.Close();
        }
    }

    public static void DeleteSaves()
    {
        if (IsExist(saveFile))
        {
            File.Delete(saveFile);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SaveLoadManager", menuName = "ScriptableObjects/Manager/SaveLoadManager", order = 1)]
public class SaveLoadManager : ScriptableObject
{
    private string saveDirectory;
    private string saveFile;

    private void OnEnable()
    {
        saveDirectory = Application.persistentDataPath + "/Saves";
        saveFile = saveDirectory + "/LastSave.txt";
    }

    private bool IsExist(string filePath)
    {
        return File.Exists(filePath) || Directory.Exists(filePath);
    }

    /*    public void SaveGame()
        {
            if (!IsExist(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = new FileStream(saveFile, FileMode.Create);
            *//*ff
            string json = JsonUtility.ToJson(GameManager.Instance.PlayerManager.PlayerStats);*//*
            bf.Serialize(file, GameManager.Instance.PlayerManager.PlayerStats);
            file.Close();
        }

        public void LoadGame()
        {
            if (!IsExist(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            BinaryFormatter bf = new BinaryFormatter();

            if (IsExist(saveFile))
            {
                FileStream file = new FileStream(saveFile, FileMode.Open);
                GameManager.Instance.PlayerManager.PlayerStats = bf.Deserialize(file) as PlayerStats;
                *//*JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), GameManager.Instance.PlayerManager.PlayerStats);*//*
                file.Close();
            }
        }*/


    public void SaveGame()
    {
        if (!IsExist(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(saveFile);

        string json = JsonUtility.ToJson(GameManager.Instance.PlayerManager.PlayerStats);
        bf.Serialize(file, json);
        file.Close();
    }

    public void LoadGame()
    {
        if (!IsExist(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        BinaryFormatter bf = new BinaryFormatter();

        if (IsExist(saveFile))
        {
            FileStream file = File.Open(saveFile, FileMode.Open);
            JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), GameManager.Instance.PlayerManager.PlayerStats);
            file.Close();
        }
    }

    /*    public void SaveGame()
        {
            if (!IsExist(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            string jsonData = JsonUtility.ToJson(GameManager.Instance.PlayerManager.PlayerStats);
            File.WriteAllText(saveFile, jsonData);
        }

        public void LoadGame()
        {
            if (IsExist(saveFile))
            {
                string jsonData = File.ReadAllText(saveFile);
                JsonUtility.FromJsonOverwrite(jsonData, GameManager.Instance.PlayerManager.PlayerStats);
            }
        }*/


    public void DeleteSaves()
    {
        if (IsExist(saveFile))
        {
            File.Delete(saveFile);
        }
    }
}

using System;
using UnityEngine;

namespace Systems.Game
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/Settings")]
    public class GameSettings : ScriptableObject
    {
        public PlayerManagerSettings playerManagerSettings;
        public MenuManagerSettings menuManagerSettings;
        public DialogueManagerSettings dialogueManagerSettings;
    }
    
    [Serializable]
    public class MenuManagerSettings
    {
        public string startSceneName;
    }
        
    [Serializable]
    public class DialogueManagerSettings
    {
        [Header("Dialogue UI")]
        public float typingSpeed = 0.04f;
        
        [Header("Audio")]
        public DialogueAudioInfoSO defaultAudioInfo;
        public DialogueAudioInfoSO[] audioInfos;
        public bool makePredictable;
    }
        
    [Serializable]
    public class PlayerManagerSettings
    {
        public PlayerStats startingPlayerStats;
        public GameObject playerPrefab;
        public string spawnTag = "Respawn";
    }
}
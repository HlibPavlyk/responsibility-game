using System;
using UnityEngine;
using Features.Audio;

namespace Systems.Game
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/Settings")]
    public class GameSettings : ScriptableObject
    {
        public PlayerManagerSettings playerManagerSettings;
        public MenuManagerSettings menuManagerSettings;
        public DialogueManagerSettings dialogueManagerSettings;
        public MusicManagerSettings musicManagerSettings;
        public SfxLibrarySettings sfxLibrarySettings;
        public FootstepSettings footstepSettings;
        public AudioManagerSettings audioManagerSettings;
        public GeneralSettings generalSettings;
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

    [Serializable]
    public class AudioManagerSettings
    {
        [Header("Volume Settings")]
        [Range(0f, 1f)] public float musicVolume = 0.8f;
        [Range(0f, 1f)] public float sfxVolume = 1f;
    }

    [Serializable]
    public class GeneralSettings
    {
        [Header("Localization")]
        public string language = "ua"; // "ua" for Ukrainian, "en" for English
    }
}
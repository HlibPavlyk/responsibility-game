using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Systems.Game
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/Settings")]
    public class GameSettings : ScriptableObject
    {
        public PlayerManagerSettings playerManagerSettings;
        public MenuManagerSettings menuManagerSettings;
        public DialogueManagerSettings dialogueManagerSettings;
        public Features.Audio.MusicManagerSettings musicManagerSettings;
        public Features.Audio.SfxLibrarySettings sfxLibrarySettings;
        public Features.Audio.FootstepSettings footstepSettings;
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
    public class MusicManagerSettings
    {
        [Serializable]
        public class SceneMusicEntry
        {
            public string sceneName;
            public AudioClip clip;
            [Range(0f, 1f)] public float volumeOverride = -1f; // -1 => use defaultVolume
            public bool loop = true;
        }

        [Header("Output")] public AudioMixerGroup output; // optional

        [Header("Defaults")] [Range(0f, 1f)] public float defaultVolume = 0.8f;
        [Range(0f, 5f)] public float crossfadeSeconds = 0.75f;
        public AudioClip defaultClip;

        [Header("Per-Scene Mapping")] public SceneMusicEntry[] sceneMusic;

    }
}
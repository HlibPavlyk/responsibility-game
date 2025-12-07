using System;
using System.Collections.Generic;
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
    
    
    [Serializable]
    public class MusicManagerSettings
    {
        [Serializable]
        public class MusicTrack
        {
            public string id = "track_id";
            public AudioClip clip;
            [Range(0f, 1f)] public float volume = 1f;
            public bool loop = true;
        }

        [Serializable]
        public class SceneMusicBinding
        {
            public string sceneName;
            public string trackId;
        }

        [Header("Tracks")]
        public List<MusicTrack> tracks = new List<MusicTrack>();
        public string fallbackTrackId;

        [Header("Scene Bindings")]
        public List<SceneMusicBinding> sceneBindings = new List<SceneMusicBinding>();
        public bool playOnLevelLoaded = true;

        [Header("Fades")]
        public float defaultFadeDuration = 1.5f;

        [Header("Dialogue Ducking")]
        [Range(0f, 1f)] public float dialogueDuckVolume = 0.5f;
        public float duckFadeDuration = 0.25f;
    }
    
    
    [Serializable]
    public class FootstepSettings
    {
        [Serializable]
        public class SurfaceSet
        { 
            public string surfaceTag = "Untagged"; // Tag on ground colliders
            public AudioClip[] clips;
            [Range(0f, 1f)] public float volume = 1f;
            public Vector2 pitchRange = new Vector2(1f, 1f);
        } 
        [Header("Cadence")]
        [Tooltip("Seconds between steps at speed = 1. Lower for faster cadence.")]
        public float baseInterval = 0.5f;
        public float minSpeedToStep = 0.1f;

        [Header("Raycast Ground Check")]
        public LayerMask groundMask = ~0;
        public float groundCheckDistance = 0.2f;

        [Header("Sets")]
        public SurfaceSet defaultSet;
        public List<SurfaceSet> surfaceSets = new List<SurfaceSet>();

        public SurfaceSet GetSetForTag(string tag)
        {
            foreach (var s in surfaceSets)
                if (s != null && s.surfaceTag == tag) return s;
            return defaultSet;
        }
    }
    
    
    [Serializable]
    public class SfxLibrarySettings
    {
        [Serializable]
        public class SfxEntry
        {
            public string id;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume = 1f;
            public Vector2 pitchRange = new Vector2(1f, 1f);
        } 
        public AudioMixerGroup output;
        public List<SfxEntry> entries = new List<SfxEntry>();
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Features.Audio
{
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
}
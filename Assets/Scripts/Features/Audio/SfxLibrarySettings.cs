using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; 

namespace Features.Audio
{
    [CreateAssetMenu(menuName = "Audio/SFX Library Settings")]
    public class SfxLibrarySettings : ScriptableObject
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
using System;
using System.Collections.Generic;
using UnityEngine; 

namespace Features.Audio
{
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

}
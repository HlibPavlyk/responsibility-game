using UnityEngine;
using UnityEngine.Audio; 

namespace Core.Abstractions
{
    public struct SfxOptions
    {
        public float volumeScale;
        public float pitch;
        public float spatialBlend;
        public AudioMixerGroup mixer;
        public float maxDistance;
    } 
    public interface ISfxManager
    {
        void Play(string id, float volumeScale = 1f);
        void PlayAt(string id, Vector3 position, float volumeScale = 1f);
        void PlayClip(AudioClip clip, in SfxOptions options);
        void PlayClipAt(AudioClip clip, Vector3 position, in SfxOptions options);
        void Cleanup();
    }
}
using Core.ValueObjects;
using UnityEngine;

namespace Core.Abstractions
{
    public interface ISfxManager
    {
        void Play(string id, float volumeScale = 1f);
        void PlayAt(string id, Vector3 position, float volumeScale = 1f);
        void PlayClip(AudioClip clip, in SfxOptions options);
        void PlayClipAt(AudioClip clip, Vector3 position, in SfxOptions options);
        void Cleanup();
    }
}
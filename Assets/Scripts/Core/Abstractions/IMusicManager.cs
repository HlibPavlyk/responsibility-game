using UnityEngine;

namespace Core.Abstractions
{
    public interface IMusicManager
    {
        void Initialize();
        void Play(string trackId, float? fadeDuration = null);
        void PlaySceneMusic(string sceneName, float? fadeDuration = null);
        void Stop(float? fadeDuration = null);
        void SetMasterVolume(float volume01);
        float GetMasterVolume();
        void Mute(bool mute);
        bool IsMuted();
        void Cleanup();
    }
}
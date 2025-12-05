using System.Collections.Generic;
using Core.Abstractions;
using UnityEngine;
using VContainer; 

namespace Features.Audio
{
    public class SfxManager : ISfxManager
    {
        [Inject] private readonly SfxLibrarySettings settings;
        [Inject] private readonly Systems.Game.GameSettings gameSettings;

        private class Host : MonoBehaviour {}

        private GameObject root;
        private Host host;
        private AudioSource global2D;
        private Dictionary<string, SfxLibrarySettings.SfxEntry> map;
        private bool initialized;

        private void InitializeIfNeeded()
        {
            if (initialized) return;

            if (settings == null)
            {
                Debug.LogError("SfxLibrarySettings not registered/assigned.");
                return;
            }

            map = new Dictionary<string, SfxLibrarySettings.SfxEntry>(System.StringComparer.Ordinal);
            foreach (var e in settings.entries)
            {
                if (e != null && !string.IsNullOrEmpty(e.id) && e.clip != null)
                    map[e.id] = e;
            }

            root = new GameObject("SfxManager_AudioRoot");
            Object.DontDestroyOnLoad(root);

            host = root.AddComponent<Host>();

            global2D = root.AddComponent<AudioSource>();
            global2D.playOnAwake = false;
            global2D.spatialBlend = 0f; // 2D за замовчуванням
            if (settings.output) global2D.outputAudioMixerGroup = settings.output;

            initialized = true;
        }

        public void Play(string id, float volumeScale = 1f)
        {
            InitializeIfNeeded();
            if (!initialized) return;

            if (!map.TryGetValue(id, out var e) || e.clip == null) return;

            float master = Mathf.Clamp01(gameSettings.audioManagerSettings.sfxVolume);
            
            global2D.outputAudioMixerGroup = settings.output;
            global2D.spatialBlend = 0f;
            global2D.pitch = Random.Range(e.pitchRange.x, e.pitchRange.y);
            global2D.PlayOneShot(e.clip, Mathf.Clamp01(e.volume * volumeScale * master));
        }

        public void PlayAt(string id, Vector3 position, float volumeScale = 1f)
        {
            InitializeIfNeeded();
            if (!initialized) return;

            if (!map.TryGetValue(id, out var e) || e.clip == null) return;

            float master = Mathf.Clamp01(gameSettings.audioManagerSettings.sfxVolume);

            var go = new GameObject($"SFX_{id}");
            go.transform.position = position;

            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.spatialBlend = 0f; // 2D; якщо треба 3D — встановіть 1f або керуйте через PlayClipAt
            if (settings.output) src.outputAudioMixerGroup = settings.output;

            src.pitch = Random.Range(e.pitchRange.x, e.pitchRange.y);
            src.PlayOneShot(e.clip, Mathf.Clamp01(e.volume * volumeScale * master));

            Object.Destroy(go, (e.clip.length / Mathf.Max(src.pitch, 0.01f)) + 0.1f);
        }

        public void PlayClip(AudioClip clip, in SfxOptions opt)
        {
            InitializeIfNeeded();
            if (!initialized || clip == null) return;

            float master = Mathf.Clamp01(gameSettings.audioManagerSettings.sfxVolume);

            global2D.outputAudioMixerGroup = opt.mixer ? opt.mixer : settings.output;
            global2D.spatialBlend = Mathf.Clamp01(opt.spatialBlend);
            global2D.pitch = (opt.pitch == 0f) ? 1f : opt.pitch;

            global2D.PlayOneShot(clip, Mathf.Clamp01(opt.volumeScale * master));
        }

        public void PlayClipAt(AudioClip clip, Vector3 position, in SfxOptions opt)
        {
            InitializeIfNeeded();
            if (!initialized || clip == null) return;

            var go = new GameObject("SFX_OneShot");
            go.transform.position = position;

            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.outputAudioMixerGroup = opt.mixer ? opt.mixer : settings.output;
            src.spatialBlend = Mathf.Clamp01(opt.spatialBlend);
            if (opt.spatialBlend > 0f && opt.maxDistance > 0f)
                src.maxDistance = opt.maxDistance;

            src.pitch = (opt.pitch == 0f) ? 1f : opt.pitch;

            float master = Mathf.Clamp01(gameSettings.audioManagerSettings.sfxVolume);
            src.PlayOneShot(clip, Mathf.Clamp01(opt.volumeScale * master));

            Object.Destroy(go, (clip.length / Mathf.Max(src.pitch, 0.01f)) + 0.1f);
        }

        public void Cleanup()
        {
            if (root != null) Object.Destroy(root);
            root = null;
            host = null;
            global2D = null;
            map = null;
            initialized = false;
        }
    }
}
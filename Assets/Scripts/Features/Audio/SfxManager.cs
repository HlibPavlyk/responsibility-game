using System.Collections.Generic;
using System.Linq;
using Core.Abstractions;
using Core.ValueObjects;
using Systems.Game;
using UnityEngine;
using VContainer; 

namespace Features.Audio
{
    public class SfxManager : ISfxManager
    {
        [Inject] private readonly SfxLibrarySettings _settings;
        [Inject] private readonly GameSettings _gameSettings;

        private class Host : MonoBehaviour {}

        private GameObject _root;
        private Host _host;
        private AudioSource _global2D;
        private Dictionary<string, SfxLibrarySettings.SfxEntry> _map;
        private bool _initialized;

        private void InitializeIfNeeded()
        {
            if (_initialized) return;

            if (_settings == null)
            {
                Debug.LogError("SfxLibrarySettings not registered/assigned.");
                return;
            }

            _map = _settings.entries
                .Where(e => e != null && !string.IsNullOrEmpty(e.id) && e.clip)
                .ToDictionary(e => e.id, e => e);

            _root = new GameObject("SfxManager_AudioRoot");
            Object.DontDestroyOnLoad(_root);

            _host = _root.AddComponent<Host>();

            _global2D = _root.AddComponent<AudioSource>();
            _global2D.playOnAwake = false;
            _global2D.spatialBlend = 0f;
            if (_settings.output) _global2D.outputAudioMixerGroup = _settings.output;

            _initialized = true;
        }

        public void Play(string id, float volumeScale = 1f)
        {
            InitializeIfNeeded();
            if (!_initialized) return;

            if (!_map.TryGetValue(id, out var e) || !e.clip) return;

            var master = Mathf.Clamp01(_gameSettings.audioManagerSettings.sfxVolume);
            
            _global2D.outputAudioMixerGroup = _settings.output;
            _global2D.spatialBlend = 0f;
            _global2D.pitch = Random.Range(e.pitchRange.x, e.pitchRange.y);
            _global2D.PlayOneShot(e.clip, Mathf.Clamp01(e.volume * volumeScale * master));
        }

        public void PlayAt(string id, Vector3 position, float volumeScale = 1f)
        {
            InitializeIfNeeded();
            if (!_initialized) return;

            if (!_map.TryGetValue(id, out var e) || e.clip == null) return;

            var master = Mathf.Clamp01(_gameSettings.audioManagerSettings.sfxVolume);

            var go = new GameObject($"SFX_{id}")
            {
                transform = { position = position }
            };

            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.spatialBlend = 0f;
            if (_settings.output) src.outputAudioMixerGroup = _settings.output;

            src.pitch = Random.Range(e.pitchRange.x, e.pitchRange.y);
            src.PlayOneShot(e.clip, Mathf.Clamp01(e.volume * volumeScale * master));

            Object.Destroy(go, e.clip.length / Mathf.Max(src.pitch, 0.5f) + 0.1f);
        }

        public void PlayClip(AudioClip clip, in SfxOptions opt)
        {
            InitializeIfNeeded();
            if (!_initialized || clip == null) return;

            var master = Mathf.Clamp01(_gameSettings.audioManagerSettings.sfxVolume);

            _global2D.outputAudioMixerGroup = opt.Mixer ? opt.Mixer : _settings.output;
            _global2D.spatialBlend = Mathf.Clamp01(opt.SpatialBlend);
            _global2D.pitch = opt.Pitch == 0f ? 1f : opt.Pitch;

            _global2D.PlayOneShot(clip, Mathf.Clamp01(opt.VolumeScale * master));
        }

        public void PlayClipAt(AudioClip clip, Vector3 position, in SfxOptions opt)
        {
            InitializeIfNeeded();
            if (!_initialized || !clip) return;

            var go = new GameObject("SFX_OneShot")
            {
                transform = { position = position }
            };

            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.outputAudioMixerGroup = opt.Mixer ? opt.Mixer : _settings.output;
            src.spatialBlend = Mathf.Clamp01(opt.SpatialBlend);
            if (opt is { SpatialBlend: > 0f, MaxDistance: > 0f })
                src.maxDistance = opt.MaxDistance;

            src.pitch = opt.Pitch == 0f ? 1f : opt.Pitch;

            var master = Mathf.Clamp01(_gameSettings.audioManagerSettings.sfxVolume);
            src.PlayOneShot(clip, Mathf.Clamp01(opt.VolumeScale * master));

            Object.Destroy(go, clip.length / Mathf.Max(src.pitch, 0.01f) + 0.1f);
        }

        public void Cleanup()
        {
            if (_root != null) Object.Destroy(_root);
            _root = null;
            _host = null;
            _global2D = null;
            _map = null;
            _initialized = false;
        }
    }
}
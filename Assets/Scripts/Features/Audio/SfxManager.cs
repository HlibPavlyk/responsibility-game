using System.Collections.Generic;
using Core.Abstractions;
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

            _map = new Dictionary<string, SfxLibrarySettings.SfxEntry>(System.StringComparer.Ordinal);
            foreach (var e in _settings.entries)
            {
                if (e != null && !string.IsNullOrEmpty(e.id) && e.clip != null)
                    _map[e.id] = e;
            }

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

            if (!_map.TryGetValue(id, out var e) || e.clip == null) return;

            float master = Mathf.Clamp01(_gameSettings.audioManagerSettings.sfxVolume);
            
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

            float master = Mathf.Clamp01(_gameSettings.audioManagerSettings.sfxVolume);

            var go = new GameObject($"SFX_{id}");
            go.transform.position = position;

            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.spatialBlend = 0f;
            if (_settings.output) src.outputAudioMixerGroup = _settings.output;

            src.pitch = Random.Range(e.pitchRange.x, e.pitchRange.y);
            src.PlayOneShot(e.clip, Mathf.Clamp01(e.volume * volumeScale * master));

            Object.Destroy(go, (e.clip.length / Mathf.Max(src.pitch, 0.5f)) + 0.1f);
        }

        public void PlayClip(AudioClip clip, in SfxOptions opt)
        {
            InitializeIfNeeded();
            if (!_initialized || clip == null) return;

            var master = Mathf.Clamp01(_gameSettings.audioManagerSettings.sfxVolume);

            _global2D.outputAudioMixerGroup = opt.mixer ? opt.mixer : _settings.output;
            _global2D.spatialBlend = Mathf.Clamp01(opt.spatialBlend);
            _global2D.pitch = (opt.pitch == 0f) ? 1f : opt.pitch;

            _global2D.PlayOneShot(clip, Mathf.Clamp01(opt.volumeScale * master));
        }

        public void PlayClipAt(AudioClip clip, Vector3 position, in SfxOptions opt)
        {
            InitializeIfNeeded();
            if (!_initialized || clip == null) return;

            var go = new GameObject("SFX_OneShot");
            go.transform.position = position;

            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.outputAudioMixerGroup = opt.mixer ? opt.mixer : _settings.output;
            src.spatialBlend = Mathf.Clamp01(opt.spatialBlend);
            if (opt.spatialBlend > 0f && opt.maxDistance > 0f)
                src.maxDistance = opt.maxDistance;

            src.pitch = (opt.pitch == 0f) ? 1f : opt.pitch;

            float master = Mathf.Clamp01(_gameSettings.audioManagerSettings.sfxVolume);
            src.PlayOneShot(clip, Mathf.Clamp01(opt.volumeScale * master));

            Object.Destroy(go, (clip.length / Mathf.Max(src.pitch, 0.01f)) + 0.1f);
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
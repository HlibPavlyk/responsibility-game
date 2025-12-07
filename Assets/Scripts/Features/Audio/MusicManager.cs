using System;
using System.Collections;
using System.Collections.Generic;
using Core.Abstractions;
using Core.Events;
using Systems.Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Features.Audio
{
    public class MusicManager : IMusicManager
    {
        [Inject] private readonly MusicManagerSettings settings;

        // Nested
        private class CoroutineHost : MonoBehaviour { }

        // Static shared instance/rig
        private static MusicManager _sInstance;
        private static GameObject _sSharedRig;

        // Unity objects
        private GameObject _audioRoot;
        private CoroutineHost _host;
        private AudioSource _sourceA;
        private AudioSource _sourceB;
        private AudioSource _activeSource;
        private AudioSource _idleSource;

        // State
        private float _masterVolume = 1f;
        private bool _muted;
        private float _duckFactor = 1f;

        // Data
        private Dictionary<string, MusicManagerSettings.MusicTrack> _trackLookup;
        private Dictionary<string, string> _sceneToTrack;

        // Coroutines
        private Coroutine _crossfadeRoutine;
        private Coroutine _duckRoutine;

        // Flags
        private bool _initialized;
        private bool _subscribed;
        private bool _isDisposed;
        private bool _unitySceneEventsSubscribed;

        // Initialization
        public void Initialize()
        {
            if (_sInstance != null && !ReferenceEquals(_sInstance, this))
            {
                Debug.LogWarning("[MusicManager] Duplicate Initialize ignored.");
                return;
            }

            _sInstance = this;

            if (_initialized && !_isDisposed) return;
            if (settings == null)
            {
                Debug.LogError("MusicManagerSettings is null.");
                return;
            }

            BuildLookups();
            CreateAudioRig();

            UnsubscribeEvents();
            SubscribeEvents();
            SubscribeUnitySceneEvents();

            _initialized = true;
            _subscribed = true;
            _isDisposed = false;

            if (settings.playOnLevelLoaded)
            {
                var sceneName = SceneManager.GetActiveScene().name;
                PlaySceneMusic(sceneName);
            }
        }

        // Public API
        public void Play(string trackId, float? fadeDuration = null)
        {
            if (!EnsureInitialized()) return;

            if (string.IsNullOrEmpty(trackId))
            {
                Debug.LogWarning("Play called with empty trackId");
                return;
            }

            if (!_trackLookup.TryGetValue(trackId, out var track) || track?.clip == null)
            {
                Debug.LogWarning($"Music track '{trackId}' not found or has no clip.");
                return;
            }

            StartCrossfade(track, fadeDuration ?? settings.defaultFadeDuration);
        }

        public void PlaySceneMusic(string sceneName, float? fadeDuration = null)
        {
            if (!EnsureInitialized()) return;

            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning("PlaySceneMusic called with empty scene name");
                return;
            }

            if (_sceneToTrack != null && _sceneToTrack.TryGetValue(sceneName, out var trackId))
            {
                Play(trackId, fadeDuration);
            }
            else if (!string.IsNullOrEmpty(settings.fallbackTrackId))
            {
                Debug.LogWarning($"No scene binding for '{sceneName}'. Using fallback track '{settings.fallbackTrackId}'.");
                Play(settings.fallbackTrackId, fadeDuration);
            }
        }

        public void Stop(float? fadeDuration = null)
        {
            if (!EnsureInitialized()) return;
            EnsureAudioRig();
            if (!_host || !_activeSource || !_idleSource) return;

            if (!EnsureInitialized()) return;

            var duration = Mathf.Max(0f, fadeDuration ?? settings.defaultFadeDuration);

            StopAndClearCoroutine(ref _crossfadeRoutine);

            if (_activeSource.isPlaying)
            {
                _crossfadeRoutine = _host.StartCoroutine(FadeOutAndStop(_activeSource, duration));
            }

            if (_idleSource.isPlaying)
            {
                _host.StartCoroutine(FadeOutAndStop(_idleSource, duration));
            }
        }

        public void SetMasterVolume(float volume01)
        {
            if (!EnsureInitialized()) return;
            _masterVolume = Mathf.Clamp01(volume01);
            ApplyVolumes();
        }

        public float GetMasterVolume() => _masterVolume;

        public void Mute(bool mute)
        {
            if (!EnsureInitialized()) return;
            _muted = mute;
            ApplyVolumes();
        }

        public bool IsMuted() => _muted;

        public void Cleanup()
        {
            _isDisposed = true;

            UnsubscribeUnitySceneEvents();
            UnsubscribeEvents();

            if (_host != null)
            {
                StopAndClearCoroutine(ref _crossfadeRoutine);
                StopAndClearCoroutine(ref _duckRoutine);
            }

            if (_audioRoot != null)
                UnityEngine.Object.Destroy(_audioRoot);

            ResetRigFields();

            _trackLookup = null;
            _sceneToTrack = null;
            _initialized = false;
        }

        // Scene events subscription
        private void SubscribeUnitySceneEvents()
        {
            if (_unitySceneEventsSubscribed) return;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            _unitySceneEventsSubscribed = true;
        }

        private void UnsubscribeUnitySceneEvents()
        {
            if (!_unitySceneEventsSubscribed) return;
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            _unitySceneEventsSubscribed = false;
        }

        private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            if (!EnsureInitialized()) return;
            EnsureAudioRig();

            if (!settings.playOnLevelLoaded) return;
            PlaySceneMusic(newScene.name);
        }

        // Game event subscriptions
        private void SubscribeEvents()
        {
            if (_subscribed) return;

            try
            {
                GameEvents.Dialogue.OnDialogueStarted += HandleDialogueStarted;
                GameEvents.Dialogue.OnDialogueEnded += HandleDialogueEnded;
                GameEvents.Level.LevelLoaded += HandleLevelLoaded;
                GameEvents.Level.LevelExit += HandleLevelExit;

                _subscribed = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"MusicManager failed subscribing to events: {e.Message}");
            }
        }

        private void UnsubscribeEvents()
        {
            if (!_subscribed) return;

            try
            {
                GameEvents.Dialogue.OnDialogueStarted -= HandleDialogueStarted;
                GameEvents.Dialogue.OnDialogueEnded -= HandleDialogueEnded;
                GameEvents.Level.LevelLoaded -= HandleLevelLoaded;
                GameEvents.Level.LevelExit -= HandleLevelExit;
            }
            catch
            {
                // ignored
            }

            _subscribed = false;
        }

        // Game event handlers
        private void HandleDialogueStarted()
        {
            if (!EnsureInitialized()) return;
            StartDuck(Mathf.Clamp01(settings.dialogueDuckVolume), settings.duckFadeDuration);
        }

        private void HandleDialogueEnded()
        {
            if (!EnsureInitialized()) return;
            EnsureAudioRig();
            if (!_host || !_activeSource || !_idleSource) return;

            StartDuck(1f, settings.duckFadeDuration);
        }

        private void HandleLevelLoaded(Transform _)
        {
            if (!EnsureInitialized()) return;
            EnsureAudioRig();
            if (!_host || !_activeSource || !_idleSource) return;
            if (!settings.playOnLevelLoaded) return;

            var sceneName = SceneManager.GetActiveScene().name;
            PlaySceneMusic(sceneName);
        }

        private void HandleLevelExit(string nextSceneName, string _playerSpawn)
        {
            // Optional: pre-fade to next scene's music here if desired.
        }

        // Audio rig lifecycle
        private void CreateAudioRig()
        {
            if (_sSharedRig != null && _sSharedRig)
            {
                AttachToRig(_sSharedRig);
                return;
            }

            _audioRoot = new GameObject("MusicManager_AudioRoot");
            UnityEngine.Object.DontDestroyOnLoad(_audioRoot);

            _host = _audioRoot.AddComponent<CoroutineHost>();
            _sourceA = _audioRoot.AddComponent<AudioSource>();
            _sourceB = _audioRoot.AddComponent<AudioSource>();

            _sourceA.playOnAwake = false;
            _sourceB.playOnAwake = false;

            _activeSource = _sourceA;
            _idleSource = _sourceB;

            _sSharedRig = _audioRoot;
        }

        private void AttachToRig(GameObject rig)
        {
            _audioRoot = rig;

            _host = rig.GetComponent<CoroutineHost>();
            if (!_host) _host = rig.AddComponent<CoroutineHost>();

            var sources = rig.GetComponents<AudioSource>();
            while (sources.Length < 2)
            {
                rig.AddComponent<AudioSource>();
                sources = rig.GetComponents<AudioSource>();
            }

            _sourceA = sources[0];
            _sourceB = sources[1];

            _sourceA.playOnAwake = false;
            _sourceB.playOnAwake = false;

            _activeSource = _sourceA;
            _idleSource = _sourceB;
        }

        private void EnsureAudioRig()
        {
            if (!_audioRoot || !_host || !_sourceA || !_sourceB || !_activeSource || !_idleSource)
            {
                if (_audioRoot) UnityEngine.Object.Destroy(_audioRoot);
                ResetRigFields();
                CreateAudioRig();
            }
        }

        private void ResetRigFields()
        {
            _audioRoot = null;
            _host = null;
            _sourceA = null;
            _sourceB = null;
            _activeSource = null;
            _idleSource = null;
        }

        private bool EnsureInitialized()
        {
            if (!_initialized || _isDisposed) Initialize();
            EnsureAudioRig();
            return _initialized && !_isDisposed;
        }

        // Ducking
        private void StartDuck(float to, float duration)
        {
            if (!EnsureInitialized()) return;
            EnsureAudioRig();
            if (!_host) return;

            StopAndClearCoroutine(ref _duckRoutine);
            _duckRoutine = _host.StartCoroutine(DuckRoutine(to, Mathf.Max(0f, duration)));
        }

        private IEnumerator DuckRoutine(float targetDuck, float duration)
        {
            float start = _duckFactor;
            float t = 0f;

            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                _duckFactor = Mathf.Lerp(start, targetDuck, duration <= 0f ? 1f : t / duration);
                ApplyVolumes();
                yield return null;
            }

            _duckFactor = targetDuck;
            ApplyVolumes();
            _duckRoutine = null;
        }

        // Crossfading
        private void StartCrossfade(MusicManagerSettings.MusicTrack track, float duration)
        {
            if (!EnsureInitialized()) return;
            EnsureAudioRig();
            if (!_host || !_activeSource || !_idleSource) return;

            if (track == null || track.clip == null) return;

            if (_activeSource.clip == track.clip && _activeSource.isPlaying)
            {
                _activeSource.loop = track.loop;
                ApplyVolumes();
                return;
            }

            StopAndClearCoroutine(ref _crossfadeRoutine);
            _crossfadeRoutine = _host.StartCoroutine(CrossfadeRoutine(track, Mathf.Max(0f, duration)));
        }

        private IEnumerator CrossfadeRoutine(MusicManagerSettings.MusicTrack newTrack, float duration)
        {
            _idleSource.clip = newTrack.clip;
            _idleSource.loop = newTrack.loop;
            _idleSource.time = 0f;

            var targetNewVol = EffectiveTargetVolume(newTrack.volume);
            var startOldVol = _activeSource.isPlaying ? _activeSource.volume : 0f;

            _idleSource.volume = 0f;
            _idleSource.Play();

            var t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                var k = duration <= 0f ? 1f : t / duration;

                _idleSource.volume = Mathf.Lerp(0f, targetNewVol, k);

                if (_activeSource.isPlaying)
                {
                    _activeSource.volume = Mathf.Lerp(startOldVol, 0f, k);
                }

                yield return null;
            }

            _idleSource.volume = targetNewVol;

            if (_activeSource.isPlaying)
            {
                _activeSource.Stop();
            }

            (_activeSource, _idleSource) = (_idleSource, _activeSource);

            _idleSource.clip = null;
            _idleSource.volume = 0f;

            _crossfadeRoutine = null;
        }

        private IEnumerator FadeOutAndStop(AudioSource src, float duration)
        {
            if (src == null || !src.isPlaying) yield break;

            var start = src.volume;
            var t = 0f;

            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                var k = duration <= 0f ? 1f : t / duration;
                src.volume = Mathf.Lerp(start, 0f, k);
                yield return null;
            }

            src.volume = 0f;
            src.Stop();
        }

        // Volume helpers
        private float EffectiveTargetVolume(float trackVolume)
        {
            if (_muted) return 0f;
            return Mathf.Clamp01(trackVolume) * Mathf.Clamp01(_masterVolume) * Mathf.Clamp01(_duckFactor);
        }

        private void ApplyVolumes()
        {
            if (!EnsureInitialized()) return;
            EnsureAudioRig();
            if (!_host || !_activeSource || !_idleSource) return;

            var activeTrackVol = 1f;
            if (_activeSource.clip != null)
            {
                var track = FindTrackByClip(_activeSource.clip);
                if (track != null) activeTrackVol = track.volume;
            }

            var idleTrackVol = 1f;
            if (_idleSource.clip != null)
            {
                var track = FindTrackByClip(_idleSource.clip);
                if (track != null) idleTrackVol = track.volume;
            }

            if (_activeSource.isPlaying)
            {
                _activeSource.volume = EffectiveTargetVolume(activeTrackVol);
            }

            if (_idleSource.isPlaying)
            {
                _idleSource.volume = EffectiveTargetVolume(idleTrackVol);
            }
        }

        private MusicManagerSettings.MusicTrack FindTrackByClip(AudioClip clip)
        {
            if (clip == null || _trackLookup == null) return null;

            foreach (var kv in _trackLookup)
            {
                if (kv.Value != null && kv.Value.clip == clip)
                    return kv.Value;
            }

            return null;
        }

        // Data setup
        private void BuildLookups()
        {
            _trackLookup = new Dictionary<string, MusicManagerSettings.MusicTrack>(StringComparer.Ordinal);
            _sceneToTrack = new Dictionary<string, string>(StringComparer.Ordinal);

            if (settings.tracks != null)
            {
                foreach (var t in settings.tracks)
                {
                    if (t == null || string.IsNullOrEmpty(t.id)) continue;

                    if (!_trackLookup.TryAdd(t.id, t))
                        Debug.LogWarning($"Duplicate music track id '{t.id}' in settings.");
                }
            }

            if (settings.sceneBindings != null)
            {
                foreach (var b in settings.sceneBindings)
                {
                    if (b == null || string.IsNullOrEmpty(b.sceneName) || string.IsNullOrEmpty(b.trackId)) continue;

                    if (!_sceneToTrack.ContainsKey(b.sceneName))
                        _sceneToTrack.Add(b.sceneName, b.trackId);
                    else
                        Debug.LogWarning($"Duplicate scene binding for '{b.sceneName}' in settings.");
                }
            }
        }

        // Utility
        private void StopAndClearCoroutine(ref Coroutine routine)
        {
            if (_host != null && routine != null)
            {
                _host.StopCoroutine(routine);
            }
            routine = null;
        }
    }
}
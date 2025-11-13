using System;
using System.Collections;
using System.Collections.Generic;
using Core.Abstractions;
using Core.Events;
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
        private static MusicManager sInstance;
        private static GameObject sSharedRig;

        // Unity objects
        private GameObject audioRoot;
        private CoroutineHost host;
        private AudioSource sourceA;
        private AudioSource sourceB;
        private AudioSource activeSource;
        private AudioSource idleSource;

        // State
        private float masterVolume = 1f;
        private bool muted;
        private float duckFactor = 1f;

        // Data
        private Dictionary<string, MusicManagerSettings.MusicTrack> trackLookup;
        private Dictionary<string, string> sceneToTrack;

        // Coroutines
        private Coroutine crossfadeRoutine;
        private Coroutine duckRoutine;

        // Flags
        private bool initialized;
        private bool subscribed;
        private bool isDisposed;
        private bool unitySceneEventsSubscribed;

        // Initialization
        public void Initialize()
        {
            if (sInstance != null && !ReferenceEquals(sInstance, this))
            {
                Debug.LogWarning("[MusicManager] Duplicate Initialize ignored.");
                return;
            }

            sInstance = this;

            if (initialized && !isDisposed) return;
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

            initialized = true;
            subscribed = true;
            isDisposed = false;

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

            if (!trackLookup.TryGetValue(trackId, out var track) || track?.clip == null)
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

            if (sceneToTrack != null && sceneToTrack.TryGetValue(sceneName, out var trackId))
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
            if (!host || !activeSource || !idleSource) return;

            if (!EnsureInitialized()) return;

            float duration = Mathf.Max(0f, fadeDuration ?? settings.defaultFadeDuration);

            StopAndClearCoroutine(ref crossfadeRoutine);

            if (activeSource.isPlaying)
            {
                crossfadeRoutine = host.StartCoroutine(FadeOutAndStop(activeSource, duration));
            }

            if (idleSource.isPlaying)
            {
                host.StartCoroutine(FadeOutAndStop(idleSource, duration));
            }
        }

        public void SetMasterVolume(float volume01)
        {
            if (!EnsureInitialized()) return;
            masterVolume = Mathf.Clamp01(volume01);
            ApplyVolumes();
        }

        public float GetMasterVolume() => masterVolume;

        public void Mute(bool mute)
        {
            if (!EnsureInitialized()) return;
            muted = mute;
            ApplyVolumes();
        }

        public bool IsMuted() => muted;

        public void Cleanup()
        {
            isDisposed = true;

            UnsubscribeUnitySceneEvents();
            UnsubscribeEvents();

            if (host != null)
            {
                StopAndClearCoroutine(ref crossfadeRoutine);
                StopAndClearCoroutine(ref duckRoutine);
            }

            if (audioRoot != null)
                UnityEngine.Object.Destroy(audioRoot);

            ResetRigFields();

            trackLookup = null;
            sceneToTrack = null;
            initialized = false;
        }

        // Scene events subscription
        private void SubscribeUnitySceneEvents()
        {
            if (unitySceneEventsSubscribed) return;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            unitySceneEventsSubscribed = true;
        }

        private void UnsubscribeUnitySceneEvents()
        {
            if (!unitySceneEventsSubscribed) return;
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            unitySceneEventsSubscribed = false;
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
            if (subscribed) return;

            try
            {
                GameEvents.Dialogue.OnDialogueStarted += HandleDialogueStarted;
                GameEvents.Dialogue.OnDialogueEnded += HandleDialogueEnded;
                GameEvents.Level.LevelLoaded += HandleLevelLoaded;
                GameEvents.Level.LevelExit += HandleLevelExit;

                subscribed = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"MusicManager failed subscribing to events: {e.Message}");
            }
        }

        private void UnsubscribeEvents()
        {
            if (!subscribed) return;

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

            subscribed = false;
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
            if (!host || !activeSource || !idleSource) return;

            StartDuck(1f, settings.duckFadeDuration);
        }

        private void HandleLevelLoaded(Transform _)
        {
            if (!EnsureInitialized()) return;
            EnsureAudioRig();
            if (!host || !activeSource || !idleSource) return;
            if (!settings.playOnLevelLoaded) return;

            string sceneName = SceneManager.GetActiveScene().name;
            PlaySceneMusic(sceneName);
        }

        private void HandleLevelExit(string nextSceneName, string _playerSpawn)
        {
            // Optional: pre-fade to next scene's music here if desired.
        }

        // Audio rig lifecycle
        private void CreateAudioRig()
        {
            if (sSharedRig != null && sSharedRig)
            {
                AttachToRig(sSharedRig);
                return;
            }

            audioRoot = new GameObject("MusicManager_AudioRoot");
            UnityEngine.Object.DontDestroyOnLoad(audioRoot);

            host = audioRoot.AddComponent<CoroutineHost>();
            sourceA = audioRoot.AddComponent<AudioSource>();
            sourceB = audioRoot.AddComponent<AudioSource>();

            sourceA.playOnAwake = false;
            sourceB.playOnAwake = false;

            activeSource = sourceA;
            idleSource = sourceB;

            sSharedRig = audioRoot;
        }

        private void AttachToRig(GameObject rig)
        {
            audioRoot = rig;

            host = rig.GetComponent<CoroutineHost>();
            if (!host) host = rig.AddComponent<CoroutineHost>();

            var sources = rig.GetComponents<AudioSource>();
            while (sources.Length < 2)
            {
                rig.AddComponent<AudioSource>();
                sources = rig.GetComponents<AudioSource>();
            }

            sourceA = sources[0];
            sourceB = sources[1];

            sourceA.playOnAwake = false;
            sourceB.playOnAwake = false;

            activeSource = sourceA;
            idleSource = sourceB;
        }

        private void EnsureAudioRig()
        {
            if (!audioRoot || !host || !sourceA || !sourceB || !activeSource || !idleSource)
            {
                if (audioRoot) UnityEngine.Object.Destroy(audioRoot);
                ResetRigFields();
                CreateAudioRig();
            }
        }

        private void ResetRigFields()
        {
            audioRoot = null;
            host = null;
            sourceA = null;
            sourceB = null;
            activeSource = null;
            idleSource = null;
        }

        private bool EnsureInitialized()
        {
            if (!initialized || isDisposed) Initialize();
            EnsureAudioRig();
            return initialized && !isDisposed;
        }

        // Ducking
        private void StartDuck(float to, float duration)
        {
            if (!EnsureInitialized()) return;
            EnsureAudioRig();
            if (!host) return;

            StopAndClearCoroutine(ref duckRoutine);
            duckRoutine = host.StartCoroutine(DuckRoutine(to, Mathf.Max(0f, duration)));
        }

        private IEnumerator DuckRoutine(float targetDuck, float duration)
        {
            float start = duckFactor;
            float t = 0f;

            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                duckFactor = Mathf.Lerp(start, targetDuck, duration <= 0f ? 1f : t / duration);
                ApplyVolumes();
                yield return null;
            }

            duckFactor = targetDuck;
            ApplyVolumes();
            duckRoutine = null;
        }

        // Crossfading
        private void StartCrossfade(MusicManagerSettings.MusicTrack track, float duration)
        {
            if (!EnsureInitialized()) return;
            EnsureAudioRig();
            if (!host || !activeSource || !idleSource) return;

            if (track == null || track.clip == null) return;

            if (activeSource.clip == track.clip && activeSource.isPlaying)
            {
                activeSource.loop = track.loop;
                ApplyVolumes();
                return;
            }

            StopAndClearCoroutine(ref crossfadeRoutine);
            crossfadeRoutine = host.StartCoroutine(CrossfadeRoutine(track, Mathf.Max(0f, duration)));
        }

        private IEnumerator CrossfadeRoutine(MusicManagerSettings.MusicTrack newTrack, float duration)
        {
            idleSource.clip = newTrack.clip;
            idleSource.loop = newTrack.loop;
            idleSource.time = 0f;

            float targetNewVol = EffectiveTargetVolume(newTrack.volume);
            float startOldVol = activeSource.isPlaying ? activeSource.volume : 0f;

            idleSource.volume = 0f;
            idleSource.Play();

            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float k = duration <= 0f ? 1f : t / duration;

                idleSource.volume = Mathf.Lerp(0f, targetNewVol, k);

                if (activeSource.isPlaying)
                {
                    activeSource.volume = Mathf.Lerp(startOldVol, 0f, k);
                }

                yield return null;
            }

            idleSource.volume = targetNewVol;

            if (activeSource.isPlaying)
            {
                activeSource.Stop();
            }

            var tmp = activeSource;
            activeSource = idleSource;
            idleSource = tmp;

            idleSource.clip = null;
            idleSource.volume = 0f;

            crossfadeRoutine = null;
        }

        private IEnumerator FadeOutAndStop(AudioSource src, float duration)
        {
            if (src == null || !src.isPlaying) yield break;

            float start = src.volume;
            float t = 0f;

            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float k = duration <= 0f ? 1f : t / duration;
                src.volume = Mathf.Lerp(start, 0f, k);
                yield return null;
            }

            src.volume = 0f;
            src.Stop();
        }

        // Volume helpers
        private float EffectiveTargetVolume(float trackVolume)
        {
            if (muted) return 0f;
            return Mathf.Clamp01(trackVolume) * Mathf.Clamp01(masterVolume) * Mathf.Clamp01(duckFactor);
        }

        private void ApplyVolumes()
        {
            if (!EnsureInitialized()) return;
            EnsureAudioRig();
            if (!host || !activeSource || !idleSource) return;

            float activeTrackVol = 1f;
            if (activeSource.clip != null)
            {
                var track = FindTrackByClip(activeSource.clip);
                if (track != null) activeTrackVol = track.volume;
            }

            float idleTrackVol = 1f;
            if (idleSource.clip != null)
            {
                var track = FindTrackByClip(idleSource.clip);
                if (track != null) idleTrackVol = track.volume;
            }

            if (activeSource.isPlaying)
            {
                activeSource.volume = EffectiveTargetVolume(activeTrackVol);
            }

            if (idleSource.isPlaying)
            {
                idleSource.volume = EffectiveTargetVolume(idleTrackVol);
            }
        }

        private MusicManagerSettings.MusicTrack FindTrackByClip(AudioClip clip)
        {
            if (clip == null || trackLookup == null) return null;

            foreach (var kv in trackLookup)
            {
                if (kv.Value != null && kv.Value.clip == clip)
                    return kv.Value;
            }

            return null;
        }

        // Data setup
        private void BuildLookups()
        {
            trackLookup = new Dictionary<string, MusicManagerSettings.MusicTrack>(StringComparer.Ordinal);
            sceneToTrack = new Dictionary<string, string>(StringComparer.Ordinal);

            if (settings.tracks != null)
            {
                foreach (var t in settings.tracks)
                {
                    if (t == null || string.IsNullOrEmpty(t.id)) continue;

                    if (!trackLookup.ContainsKey(t.id))
                        trackLookup.Add(t.id, t);
                    else
                        Debug.LogWarning($"Duplicate music track id '{t.id}' in settings.");
                }
            }

            if (settings.sceneBindings != null)
            {
                foreach (var b in settings.sceneBindings)
                {
                    if (b == null || string.IsNullOrEmpty(b.sceneName) || string.IsNullOrEmpty(b.trackId)) continue;

                    if (!sceneToTrack.ContainsKey(b.sceneName))
                        sceneToTrack.Add(b.sceneName, b.trackId);
                    else
                        Debug.LogWarning($"Duplicate scene binding for '{b.sceneName}' in settings.");
                }
            }
        }

        // Utility
        private void StopAndClearCoroutine(ref Coroutine routine)
        {
            if (host != null && routine != null)
            {
                host.StopCoroutine(routine);
            }
            routine = null;
        }
    }
}
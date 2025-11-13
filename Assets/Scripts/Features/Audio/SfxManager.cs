using System;
using System.Collections.Generic;
using Core.Abstractions;
using UnityEngine;
using VContainer; 

namespace Features.Audio
{
    public class SfxManager : ISfxManager
    {
        [Inject] private readonly SfxLibrarySettings settings; 
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

        map = new Dictionary<string, SfxLibrarySettings.SfxEntry>(StringComparer.Ordinal);
        foreach (var e in settings.entries)
        {
            if (e != null && !string.IsNullOrEmpty(e.id) && e.clip != null)
                map[e.id] = e;
        }

        root = new GameObject("SfxManager_AudioRoot");
        UnityEngine.Object.DontDestroyOnLoad(root);
        host = root.AddComponent<Host>();

        global2D = root.AddComponent<AudioSource>();
        global2D.playOnAwake = false;
        global2D.spatialBlend = 0f; // 2D
        if (settings.output) global2D.outputAudioMixerGroup = settings.output;

        initialized = true;
    }

    public void Play(string id, float volumeScale = 1f)
    {
        InitializeIfNeeded();
        if (!initialized) return;

        if (!map.TryGetValue(id, out var e) || e.clip == null) return;

        global2D.pitch = UnityEngine.Random.Range(e.pitchRange.x, e.pitchRange.y);
        global2D.PlayOneShot(e.clip, Mathf.Clamp01(e.volume * volumeScale));
    }

    public void PlayAt(string id, Vector3 position, float volumeScale = 1f)
    {
        InitializeIfNeeded();
        if (!initialized) return;

        if (!map.TryGetValue(id, out var e) || e.clip == null) return;

        var go = new GameObject($"SFX_{id}");
        go.transform.position = position;
        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.spatialBlend = 0f; // 2D (set to 1f for 3D if you want positional footsteps)
        if (settings.output) src.outputAudioMixerGroup = settings.output;
        src.pitch = UnityEngine.Random.Range(e.pitchRange.x, e.pitchRange.y);
        src.PlayOneShot(e.clip, Mathf.Clamp01(e.volume * volumeScale));
        UnityEngine.Object.Destroy(go, (e.clip.length / Mathf.Max(src.pitch, 0.01f)) + 0.1f);
    }

    public void Cleanup()
    {
        if (root != null) UnityEngine.Object.Destroy(root);
        root = null; host = null; global2D = null;
        map = null;
        initialized = false;
    }
}

} 
using UnityEngine;
using VContainer;
using Core.DI; 
using VContainer.Unity;
using Core.Abstractions;

namespace Features.Audio
{
    [InjectableMonoBehaviour]
    public class FootstepEmitter : MonoBehaviour
    {
        [Header("Assign if you want to override injected settings")]
        [SerializeField] private FootstepSettings overrideSettings;
        [SerializeField] private Transform footPoint;
        [SerializeField] private bool playAs2D = true; 
        [Inject] private FootstepSettings injectedSettings;
        [Inject] private ISfxManager sfx;

    private FootstepSettings settings;
    private Rigidbody2D rb;
    private float timer;

    private void Awake()
    {
        settings = overrideSettings != null ? overrideSettings : injectedSettings;
        rb = GetComponentInParent<Rigidbody2D>() ?? GetComponent<Rigidbody2D>();

        if (sfx == null)
        {
            var scope = GetComponentInParent<LifetimeScope>() ?? FindFirstObjectByType<GlobalLifetimeScope>();
            if (scope != null)
            {
                try { sfx = scope.Container.Resolve<ISfxManager>(); }
                catch { /* ignore */ }
            }
            if (sfx == null)
                Debug.LogError($"FootstepEmitter on {name}: ISfxManager not injected/resolved. Check DI setup.");
        }
    }

    private void Update()
    {
        if (settings == null) return;

        float speed = rb ? rb.linearVelocity.magnitude : 0f;
        timer -= Time.deltaTime * Mathf.Max(1f, speed); // faster speed = faster cadence

        if (speed < settings.minSpeedToStep || !IsGrounded()) return;

        if (timer <= 0f)
        {
            PlayStep();
            timer = Mathf.Max(0.01f, settings.baseInterval);
        }
    }

    private bool IsGrounded()
    {
        Vector3 origin = footPoint ? footPoint.position : transform.position;
        var hit = Physics2D.Raycast(origin, Vector2.down, settings.groundCheckDistance, settings.groundMask);
        return hit.collider != null;
    }

    private void PlayStep()
    {
        if (sfx == null) return;

        Vector3 origin = footPoint ? footPoint.position : transform.position;
        var hit = Physics2D.Raycast(origin, Vector2.down, settings.groundCheckDistance, settings.groundMask);
        string tag = (hit.collider != null) ? hit.collider.tag : "Untagged";

        var set = settings.GetSetForTag(tag);
        if (set == null || set.clips == null || set.clips.Length == 0) return;

        var clip = set.clips[Random.Range(0, set.clips.Length)];
        if (clip == null) return;

        var opt = new SfxOptions
        {
            volumeScale = set.volume,
            pitch = Random.Range(set.pitchRange.x, set.pitchRange.y),
            spatialBlend = playAs2D ? 0f : 1f,
            mixer = null
        };
        sfx.PlayClipAt(clip, origin, opt);
    }
}

} 
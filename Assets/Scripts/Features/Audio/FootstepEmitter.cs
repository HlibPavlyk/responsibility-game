using System.Linq;
using UnityEngine;
using VContainer;
using Core.DI; 
using Core.Abstractions;
using Core.ValueObjects;
using Systems.Game;

namespace Features.Audio
{
    [InjectableMonoBehaviour]
    public class FootstepEmitter : MonoBehaviour
    {
        [Header("Assign if you want to override injected settings")] [SerializeField]
        private Transform footPoint;

        [SerializeField] private bool playAs2D = true;
        [Inject] private ISfxManager _sfx;
        [Inject] private FootstepSettings _settings;
        private Rigidbody2D _rb;
        private float _timer;

        [Inject]
        public void PostInjectInitialize()
        {
            _rb = GetComponentInParent<Rigidbody2D>() ?? GetComponent<Rigidbody2D>();
        }


        private void Update()
        {
            if (_settings == null) return;

            var speed = _rb ? _rb.linearVelocity.magnitude : 0f;
            _timer -= Time.deltaTime * Mathf.Max(1f, speed); // faster speed = faster cadence

            if (speed < _settings.minSpeedToStep || !IsGrounded()) return;

            if (_timer <= 0f)
            {
                PlayStep();
                _timer = Mathf.Max(0.01f, _settings.baseInterval);
            }
        }

        private bool IsGrounded()
        {
            var origin = footPoint ? footPoint.position : transform.position;
            var hit = Physics2D.Raycast(origin, Vector2.down, _settings.groundCheckDistance, _settings.groundMask);
            return hit.collider;
        }

        private void PlayStep()
        {
            if (_sfx == null) return;

            var origin = footPoint ? footPoint.position : transform.position;
            var hit = Physics2D.Raycast(origin, Vector2.down, _settings.groundCheckDistance, _settings.groundMask);
            
            if (!hit.collider) return;
            
            var colliderTag = hit.collider ? hit.collider.tag : "Untagged";

            var set = GetSetForTag(colliderTag);
            if (set?.clips == null || set.clips.Length == 0) return;

            var clip = set.clips[Random.Range(0, set.clips.Length)];
            if (!clip) return;

            var opt = new SfxOptions
            {
                VolumeScale = set.volume,
                Pitch = Random.Range(set.pitchRange.x, set.pitchRange.y),
                SpatialBlend = playAs2D ? 0f : 1f,
                Mixer = null
            };
            _sfx.PlayClipAt(clip, origin, opt);
        }
        
        private FootstepSettings.SurfaceSet GetSetForTag(string componentTag)
        {
            return _settings.surfaceSets.FirstOrDefault(s => s != null && s.surfaceTag == componentTag)
                   ?? _settings.defaultSet;
        }
    }

}
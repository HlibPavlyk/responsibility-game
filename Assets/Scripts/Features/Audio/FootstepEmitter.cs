using UnityEngine;
using VContainer;
using Core.DI; 
using Core.Abstractions;
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
            return hit.collider != null;
        }

        private void PlayStep()
        {
            if (_sfx == null) return;

            var origin = footPoint ? footPoint.position : transform.position;
            var hit = Physics2D.Raycast(origin, Vector2.down, _settings.groundCheckDistance, _settings.groundMask);
            var tag = (hit.collider != null) ? hit.collider.tag : "Untagged";

            var set = _settings.GetSetForTag(tag);
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
            _sfx.PlayClipAt(clip, origin, opt);
        }
    }

}
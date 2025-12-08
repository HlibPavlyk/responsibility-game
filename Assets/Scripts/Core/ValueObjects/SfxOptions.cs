using UnityEngine.Audio;

namespace Core.ValueObjects
{
    public struct SfxOptions
    {
        public float VolumeScale;
        public float Pitch;
        public float SpatialBlend;
        public AudioMixerGroup Mixer;
        public float MaxDistance;
    } 
}
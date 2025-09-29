using System;
using UnityEngine;

namespace Core.DI
{
    [Serializable]
    public class DialogueManagerSettings
    {
        [Header("Dialogue UI")]
        public float typingSpeed = 0.04f;
        
        [Header("Audio")]
        public DialogueAudioInfoSO defaultAudioInfo;
        public DialogueAudioInfoSO[] audioInfos;
        public bool makePredictable;
    }
}
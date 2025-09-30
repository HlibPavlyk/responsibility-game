using UnityEngine;

namespace Core.Abstractions
{
    public interface IDialogueManager
    {
        void Initialize(GameObject canvas);
        void EnterDialogueMode(TextAsset inkJSON);
        void DialogUpdate();
        void SetCharacterName(string name);
        void SetCharacterAnimatorState(string stateName);
        void SetCharacterLayoutPosition(string layoutPosition);
        void SetCurrentAudioInfo(string id);
        void Cleanup();
    }
}

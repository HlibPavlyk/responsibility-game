using Core.Abstractions;
using Core.DI;
using UnityEngine;
using VContainer;

namespace Features.Dialogue
{
    [InjectableMonoBehaviour]
    public class DialogInitiate : MonoBehaviour
    {
        [Inject] private readonly IDialogueManager _dialogueManager;

        private void Start()
        {
            _dialogueManager.Initialize(gameObject);
        }

        private void FixedUpdate()
        {
            _dialogueManager.DialogUpdate();
        }
    }
}

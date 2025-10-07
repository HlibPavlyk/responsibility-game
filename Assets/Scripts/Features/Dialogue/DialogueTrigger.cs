using Core.Abstractions;
using Core.DI;
using Systems.Game;
using UnityEngine;
using VContainer;

namespace Features.Dialogue
{
    [InjectableMonoBehaviour]
    public class DialogueTrigger : MonoBehaviour
    {
        // serialized fields
        [Header("VisualCue")]
        [SerializeField] private GameObject visualCue;

        [Header("Ink JSON")]
        [SerializeField] private TextAsset inkJson;
    
        // injected dependencies
        [Inject] private readonly IDialogueManager _dialogueManager;
        [Inject] private readonly IInputManager _inputManager;
        [Inject] private readonly GameState _gameState;
    
        // internal fields
        private bool _playerInRange;
        
        // constants
        private const string PlayerTag = "Player";
        
        private void Awake()
        {
            _playerInRange = false;
            visualCue.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (_playerInRange && !_gameState.isDialoguePlaying)
            {
                visualCue.SetActive(true);
                if (_inputManager.GetInteractPressed())
                {
                    _dialogueManager.EnterDialogueMode(inkJson);
                }
            }
            else
            {
                visualCue.SetActive(false);
            }
        }
        private void OnTriggerEnter2D(Collider2D gameCollider)
        {
            if (gameCollider.gameObject.CompareTag(PlayerTag))
            {
                _playerInRange = true;
            }
        }

        private void OnTriggerExit2D(Collider2D gameCollider)
        {
            if (gameCollider.gameObject.CompareTag(PlayerTag))
            {
                _playerInRange = false;
            }
        }
    }
}

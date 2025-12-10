using Core.Abstractions;
using Core.DI;
using Core.Events;
using Features.Story.Actions;
using Features.Story.Conditions;
using Systems.Game;
using UnityEngine;
using VContainer;

namespace Features.Story
{
    [InjectableMonoBehaviour]
    public class StoryTrigger : MonoBehaviour
    {
        [Header("Trigger Identity")]
        [SerializeField] private string triggerID;
        [SerializeField] private bool oneTimeOnly = true;
        [SerializeField] private bool startEnabled = true;

        [Header("Visual Cue")]
        [SerializeField] private GameObject visualCue;

        [Header("Conditions")]
        [SerializeReference] private StoryCondition[] conditions;

        [Header("Actions")]
        [SerializeReference] private StoryAction[] actions;

        // Injected dependencies
        [Inject] private readonly IStoryManager _storyManager;
        [Inject] private readonly IDialogueManager _dialogueManager;
        [Inject] private readonly ILevelManager _levelManager;
        [Inject] private readonly IInputManager _inputManager;
        [Inject] private readonly GameState _gameState;

        // Internal state
        private bool _playerInRange;
        private bool _isEnabled;
        private const string PlayerTag = "Player";

        private void Awake()
        {
            if (string.IsNullOrEmpty(triggerID))
            {
                Debug.LogError($"StoryTrigger on GameObject '{gameObject.name}' has no triggerID set!");
            }

            _playerInRange = false;
            _isEnabled = startEnabled;

            if (visualCue != null)
                visualCue.SetActive(false);

            // Register with StoryManager
            if (_storyManager != null)
            {
                _storyManager.RegisterTrigger(triggerID, this);
            }
        }

        private void OnDestroy()
        {
            // Unregister from StoryManager
            if (_storyManager != null)
            {
                _storyManager.UnregisterTrigger(triggerID);
            }
        }

        private void FixedUpdate()
        {
            // Don't show visual cue or accept input if disabled or dialogue is playing
            if (!_isEnabled || !_playerInRange || _gameState.isDialoguePlaying)
            {
                if (visualCue != null)
                    visualCue.SetActive(false);
                return;
            }

            // Check if trigger can activate
            if (!CanActivate())
            {
                if (visualCue != null)
                    visualCue.SetActive(false);
                return;
            }

            // Show visual cue
            if (visualCue != null)
                visualCue.SetActive(true);

            // Check for interaction input
            if (_inputManager.GetInteractPressed())
            {
                Activate();
            }
        }

        private bool CanActivate()
        {
            // Check one-time constraint
            if (oneTimeOnly && _storyManager.HasTriggerFired(triggerID))
            {
                return false;
            }

            // Evaluate conditions
            if (conditions != null && conditions.Length > 0)
            {
                foreach (var condition in conditions)
                {
                    if (condition != null && !condition.Evaluate(_storyManager))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void Activate()
        {
            // Mark as fired if one-time
            if (oneTimeOnly)
            {
                _storyManager.MarkTriggerFired(triggerID);
            }

            // Create action context
            var context = new StoryActionContext
            {
                StoryManager = _storyManager,
                DialogueManager = _dialogueManager,
                LevelManager = _levelManager,
                TriggerID = triggerID
            };

            // Execute actions
            if (actions != null)
            {
                foreach (var action in actions)
                {
                    action?.Execute(context);
                }
            }

            // Fire event
            GameEvents.Story.OnTriggerActivated?.Invoke(triggerID);

            // Hide visual cue after activation
            if (visualCue != null)
                visualCue.SetActive(false);
        }

        public void Enable()
        {
            _isEnabled = true;
        }

        public void Disable()
        {
            _isEnabled = false;
            if (visualCue != null)
                visualCue.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D gameCollider)
        {
            if (gameCollider.CompareTag(PlayerTag))
            {
                _playerInRange = true;
            }
        }

        private void OnTriggerExit2D(Collider2D gameCollider)
        {
            if (gameCollider.CompareTag(PlayerTag))
            {
                _playerInRange = false;
            }
        }
    }
}

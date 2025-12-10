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
    /// <summary>
    /// Automatic Story Trigger - activates immediately when player enters the collider.
    /// No interaction input required, no visual cue shown.
    /// Perfect for cutscenes, automatic dialogues, or event triggers.
    /// </summary>
    [InjectableMonoBehaviour]
    public class AutoStoryTrigger : MonoBehaviour
    {
        [Header("Trigger Identity")]
        [SerializeField] private string triggerID;
        [SerializeField] private bool oneTimeOnly = true;
        [SerializeField] private bool startEnabled = true;

        [Header("Activation Delay")]
        [Tooltip("Delay in seconds before trigger activates after player enters")]
        [SerializeField] private float activationDelay = 0f;

        [Header("Conditions")]
        [SerializeReference] private StoryCondition[] conditions;

        [Header("Actions")]
        [SerializeReference] private StoryAction[] actions;

        // Injected dependencies
        [Inject] private readonly IStoryManager _storyManager;
        [Inject] private readonly IDialogueManager _dialogueManager;
        [Inject] private readonly ILevelManager _levelManager;
        [Inject] private readonly GameState _gameState;

        // Internal state
        private bool _isEnabled;
        private bool _isWaitingToActivate;
        private float _activationTimer;
        private const string PlayerTag = "Player";

        private void Awake()
        {
            if (string.IsNullOrEmpty(triggerID))
            {
                Debug.LogError($"AutoStoryTrigger on GameObject '{gameObject.name}' has no triggerID set!");
            }

            _isEnabled = startEnabled;
            _isWaitingToActivate = false;
            _activationTimer = 0f;

            // Register with StoryManager
            if (_storyManager != null)
            {
                _storyManager.RegisterTrigger(triggerID, null); // AutoTrigger doesn't need Enable/Disable from manager
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

        private void Update()
        {
            // Handle activation delay
            if (_isWaitingToActivate)
            {
                _activationTimer -= Time.deltaTime;

                if (_activationTimer <= 0f)
                {
                    _isWaitingToActivate = false;
                    TryActivate();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D gameCollider)
        {
            if (!gameCollider.CompareTag(PlayerTag))
                return;

            if (!_isEnabled)
                return;

            if (_gameState.isDialoguePlaying)
                return;

            // Check if can activate
            if (!CanActivate())
                return;

            // Start activation (with optional delay)
            if (activationDelay > 0f)
            {
                _isWaitingToActivate = true;
                _activationTimer = activationDelay;
            }
            else
            {
                TryActivate();
            }
        }

        private void OnTriggerExit2D(Collider2D gameCollider)
        {
            if (gameCollider.CompareTag(PlayerTag))
            {
                // Cancel pending activation if player leaves before delay expires
                _isWaitingToActivate = false;
                _activationTimer = 0f;
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

        private void TryActivate()
        {
            // Double-check conditions before activating (in case something changed during delay)
            if (!CanActivate())
                return;

            Activate();
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
        }

        public void Enable()
        {
            _isEnabled = true;
        }

        public void Disable()
        {
            _isEnabled = false;
            _isWaitingToActivate = false;
            _activationTimer = 0f;
        }
    }
}

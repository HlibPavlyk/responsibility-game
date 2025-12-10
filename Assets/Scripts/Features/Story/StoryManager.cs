using System.Collections.Generic;
using Core.Abstractions;
using Core.Events;
using Features.Story.Conditions;
using Systems.Game;
using UnityEngine;
using VContainer;

namespace Features.Story
{
    public class StoryManager : IStoryManager
    {
        private readonly StoryState _storyState;
        private readonly Dictionary<string, StoryTrigger> _triggerRegistry = new Dictionary<string, StoryTrigger>();

        [Inject]
        public StoryManager(StoryState storyState)
        {
            _storyState = storyState;
        }

        // Flag management
        public bool CheckFlag(string flagName)
        {
            return _storyState.HasFlag(flagName);
        }

        public void SetFlag(string flagName, bool value = true)
        {
            if (value)
            {
                _storyState.SetFlag(flagName);
                GameEvents.Story.OnFlagSet?.Invoke(flagName);
            }
            else
            {
                _storyState.ClearFlag(flagName);
                GameEvents.Story.OnFlagCleared?.Invoke(flagName);
            }
        }

        public void ClearFlag(string flagName)
        {
            _storyState.ClearFlag(flagName);
            GameEvents.Story.OnFlagCleared?.Invoke(flagName);
        }

        // Condition evaluation
        public bool EvaluateConditions(StoryCondition[] conditions)
        {
            if (conditions == null || conditions.Length == 0)
                return true;

            foreach (var condition in conditions)
            {
                if (condition != null && !condition.Evaluate(this))
                {
                    return false;
                }
            }

            return true;
        }

        // Trigger management
        public bool CanTriggerActivate(string triggerID, StoryCondition[] conditions)
        {
            // Check if trigger has already fired (handled by StoryTrigger itself)
            // This method primarily evaluates conditions
            return EvaluateConditions(conditions);
        }

        public void MarkTriggerFired(string triggerID)
        {
            _storyState.MarkTriggered(triggerID);
        }

        public bool HasTriggerFired(string triggerID)
        {
            return _storyState.HasTriggered(triggerID);
        }

        // Trigger registry for Enable/Disable actions
        public void RegisterTrigger(string triggerID, StoryTrigger trigger)
        {
            if (string.IsNullOrEmpty(triggerID))
            {
                Debug.LogWarning($"Attempted to register trigger with empty ID on GameObject: {trigger.gameObject.name}");
                return;
            }

            if (_triggerRegistry.ContainsKey(triggerID))
            {
                Debug.LogWarning($"Trigger with ID '{triggerID}' is already registered. Existing: {_triggerRegistry[triggerID].gameObject.name}, New: {trigger.gameObject.name}");
                return;
            }

            _triggerRegistry[triggerID] = trigger;
        }

        public void UnregisterTrigger(string triggerID)
        {
            if (string.IsNullOrEmpty(triggerID))
                return;

            _triggerRegistry.Remove(triggerID);
        }

        public void EnableTrigger(string triggerID)
        {
            if (_triggerRegistry.TryGetValue(triggerID, out var trigger))
            {
                trigger.Enable();
            }
            else
            {
                Debug.LogWarning($"Attempted to enable trigger '{triggerID}' but it's not registered");
            }
        }

        public void DisableTrigger(string triggerID)
        {
            if (_triggerRegistry.TryGetValue(triggerID, out var trigger))
            {
                trigger.Disable();
            }
            else
            {
                Debug.LogWarning($"Attempted to disable trigger '{triggerID}' but it's not registered");
            }
        }
    }
}

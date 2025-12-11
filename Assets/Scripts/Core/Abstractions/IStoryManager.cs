using Features.Story.Conditions;

namespace Core.Abstractions
{
    public interface IStoryManager
    {
        // Flag management
        bool CheckFlag(string flagName);
        void SetFlag(string flagName, bool value = true);
        void ClearFlag(string flagName);

        // Condition evaluation
        bool EvaluateConditions(StoryCondition[] conditions);

        // Trigger management
        bool CanTriggerActivate(string triggerID, StoryCondition[] conditions);
        void MarkTriggerFired(string triggerID);
        bool HasTriggerFired(string triggerID);

        // Trigger registry (for Enable/Disable actions)
        void RegisterTrigger(string triggerID, Features.Story.StoryTrigger trigger);
        void UnregisterTrigger(string triggerID);
        void EnableTrigger(string triggerID);
        void DisableTrigger(string triggerID);
    }
}

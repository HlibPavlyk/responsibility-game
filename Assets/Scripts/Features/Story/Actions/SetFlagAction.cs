using System;
using UnityEngine;

namespace Features.Story.Actions
{
    [Serializable]
    public class SetFlagAction : StoryAction
    {
        [SerializeField] private string flagName;
        [SerializeField] private bool value = true;

        public override void Execute(StoryActionContext context)
        {
            if (string.IsNullOrEmpty(flagName))
            {
                Debug.LogWarning($"SetFlagAction: flagName is empty for trigger {context.TriggerID}");
                return;
            }

            context.StoryManager.SetFlag(flagName, value);
        }
    }
}

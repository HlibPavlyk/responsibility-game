using System;
using UnityEngine;

namespace Features.Story.Actions
{
    [Serializable]
    public class DisableTriggerAction : StoryAction
    {
        [SerializeField] private string targetTriggerID;

        public override void Execute(StoryActionContext context)
        {
            if (string.IsNullOrEmpty(targetTriggerID))
            {
                Debug.LogWarning($"DisableTriggerAction: targetTriggerID is empty for trigger {context.TriggerID}");
                return;
            }

            context.StoryManager.DisableTrigger(targetTriggerID);
        }
    }
}

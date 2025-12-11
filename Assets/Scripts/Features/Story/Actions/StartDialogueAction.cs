using System;
using UnityEngine;

namespace Features.Story.Actions
{
    [Serializable]
    public class StartDialogueAction : StoryAction
    {
        [SerializeField] private TextAsset inkJson;

        public override void Execute(StoryActionContext context)
        {
            if (inkJson != null && context.DialogueManager != null)
            {
                context.DialogueManager.EnterDialogueMode(inkJson);
            }
            else if (inkJson == null)
            {
                Debug.LogWarning($"StartDialogueAction: inkJson is null for trigger {context.TriggerID}");
            }
        }
    }
}

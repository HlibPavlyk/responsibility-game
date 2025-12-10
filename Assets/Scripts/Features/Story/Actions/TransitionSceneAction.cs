using System;
using UnityEngine;

namespace Features.Story.Actions
{
    [Serializable]
    public class TransitionSceneAction : StoryAction
    {
        [SerializeField] private string sceneName;
        [SerializeField] private string spawnPointName;

        public override void Execute(StoryActionContext context)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning($"TransitionSceneAction: sceneName is empty for trigger {context.TriggerID}");
                return;
            }

            if (context.LevelManager != null)
            {
                context.LevelManager.OnLevelExit(sceneName, spawnPointName);
            }
        }
    }
}

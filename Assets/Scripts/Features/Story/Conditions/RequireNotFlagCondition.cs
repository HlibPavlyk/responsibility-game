using System;
using Core.Abstractions;
using UnityEngine;

namespace Features.Story.Conditions
{
    [Serializable]
    public class RequireNotFlagCondition : StoryCondition
    {
        [SerializeField] private string flagName;

        public override bool Evaluate(IStoryManager storyManager)
        {
            return !storyManager.CheckFlag(flagName);
        }
    }
}

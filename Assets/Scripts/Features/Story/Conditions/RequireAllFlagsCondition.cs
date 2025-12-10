using System;
using Core.Abstractions;
using UnityEngine;

namespace Features.Story.Conditions
{
    [Serializable]
    public class RequireAllFlagsCondition : StoryCondition
    {
        [SerializeField] private string[] flagNames;

        public override bool Evaluate(IStoryManager storyManager)
        {
            if (flagNames == null || flagNames.Length == 0)
                return true;

            foreach (var flag in flagNames)
            {
                if (!storyManager.CheckFlag(flag))
                    return false;
            }
            return true;
        }
    }
}

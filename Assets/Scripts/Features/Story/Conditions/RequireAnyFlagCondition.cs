using System;
using Core.Abstractions;
using UnityEngine;

namespace Features.Story.Conditions
{
    [Serializable]
    public class RequireAnyFlagCondition : StoryCondition
    {
        [SerializeField] private string[] flagNames;

        public override bool Evaluate(IStoryManager storyManager)
        {
            if (flagNames == null || flagNames.Length == 0)
                return false;

            foreach (var flag in flagNames)
            {
                if (storyManager.CheckFlag(flag))
                    return true;
            }
            return false;
        }
    }
}

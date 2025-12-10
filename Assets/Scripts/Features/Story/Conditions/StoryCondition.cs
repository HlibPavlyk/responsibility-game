using System;
using Core.Abstractions;

namespace Features.Story.Conditions
{
    [Serializable]
    public abstract class StoryCondition
    {
        public abstract bool Evaluate(IStoryManager storyManager);
    }
}

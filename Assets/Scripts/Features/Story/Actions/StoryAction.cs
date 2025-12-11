using System;

namespace Features.Story.Actions
{
    [Serializable]
    public abstract class StoryAction
    {
        public abstract void Execute(StoryActionContext context);
    }
}

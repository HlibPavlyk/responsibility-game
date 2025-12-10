using Core.Abstractions;

namespace Features.Story.Actions
{
    public class StoryActionContext
    {
        public IStoryManager StoryManager;
        public IDialogueManager DialogueManager;
        public ILevelManager LevelManager;
        public string TriggerID;
    }
}

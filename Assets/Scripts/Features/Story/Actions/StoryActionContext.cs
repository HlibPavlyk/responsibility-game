using Core.Abstractions;
using Systems.Game;
using UnityEngine;

namespace Features.Story.Actions
{
    public class StoryActionContext
    {
        public IStoryManager StoryManager;
        public IDialogueManager DialogueManager;
        public ISfxManager SfxManager;
        public GameState GameState;
        public MonoBehaviour Runner;
        public string TriggerID;
    }
}

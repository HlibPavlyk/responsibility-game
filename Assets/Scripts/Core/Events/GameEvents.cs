using System;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Events
{
    public static class GameEvents
    {
        public static class Level
        {
            public static UnityAction<Transform> levelLoaded;
            public static UnityAction<string, string> levelExit;
        }
        
        public class Player
        {
            public static UnityAction<Transform> onPlayerSpawned;
            public static UnityAction onPlayerDespawned;
        }
        
        public static class Dialogue
        {
            public static UnityAction OnDialogueStarted;
            public static UnityAction OnDialogueEnded;
            public static UnityAction<string> OnSpeakerChanged;
        }
    }
}
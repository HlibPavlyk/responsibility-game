using UnityEngine;
using UnityEngine.Events;

namespace Core.Events
{
    public static class GameEvents
    {
        public static class Level
        {
            public static UnityAction<Transform> LevelLoaded;
            public static UnityAction<string, string> LevelExit;
        }
        
        public class Player
        {
            public static UnityAction<Transform> OnPlayerSpawned;
            public static UnityAction OnPlayerDespawned;
        }
        
        public static class Dialogue
        {
            public static UnityAction OnDialogueStarted;
            public static UnityAction OnDialogueEnded;
            public static UnityAction<string> OnSpeakerChanged;
        }
        
        public static class Game
        {
            public static UnityAction OnGamePaused;
            public static UnityAction OnGameResumed;
        }
    }
}
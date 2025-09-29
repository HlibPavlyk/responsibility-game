using Core.DI;
using UnityEngine;

namespace ResponsibilityGame.Core.DI
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/Settings")]
    public class GameSettings : ScriptableObject
    {
        public PlayerManagerSettings PlayerManagerSettings;
        public MenuManagerSettings MenuManagerSettings;
        public DialogueManagerSettings DialogueManagerSettings;
        /*public GameState GameState;*/
    }
}
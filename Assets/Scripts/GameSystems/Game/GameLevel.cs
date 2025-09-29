using Core.Events;
using UnityEngine;

namespace GameSystems.Game
{
    public class GameLevel : MonoBehaviour
    {
        public Transform defaultPlayerSpawn;

        // Start the game level
        void Start()
        {
            GameEvents.Level.levelLoaded.Invoke(defaultPlayerSpawn);
        }

    }
}

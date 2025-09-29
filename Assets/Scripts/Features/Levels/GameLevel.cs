using Core.Events;
using UnityEngine;

namespace Features.Levels
{
    public class GameLevel : MonoBehaviour
    {
        public Transform defaultPlayerSpawn;

        // Start the game level
        void Start()
        {
            GameEvents.Level.LevelLoaded.Invoke(defaultPlayerSpawn);
        }

    }
}

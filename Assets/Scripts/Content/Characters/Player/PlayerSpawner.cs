using Core.Events;
using Core.Interfaces;
using UnityEngine;
using VContainer;

namespace Content.Characters.Player
{
    public class PlayerSpawner : MonoBehaviour
    {
        [Inject] private IPlayerManager playerManager;

        private void OnEnable()
        {
            GameEvents.Level.levelLoaded += playerManager.SpawnPlayer;
        }

        private void OnDisable()
        {
            GameEvents.Level.levelLoaded -= playerManager.SpawnPlayer;
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
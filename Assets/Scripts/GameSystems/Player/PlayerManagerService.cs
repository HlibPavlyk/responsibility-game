using Core.DI;
using Core.Events;
using Core.Interfaces;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GameSystems.Player
{
    public class PlayerManagerService : IPlayerManager
    {
        /*private GameObject activePlayer { get; set; }
        private PlayerStats playerStats { get; set; }
        private GameState gameState { get; set; }*/
        
        /*private string spawnTag { get; set; }
        private GameObject playerPrefab { get; set; }*/

        [Inject] private PlayerManagerSettings settings;
        [Inject] private GameState gameState;
        [Inject] private IObjectResolver container;
        /*public PlayerManagerService(PlayerManagerSettings settings, GameState gameState)
        {
            PlayerStats = settings.StartingPlayerStats;
            playerPrefab = settings.PlayerPrefab;
            spawnTag = settings.SpawnTag;
            GameState = gameState;
        }*/

        public void SpawnPlayer(Transform defaultSpawnTransform)
        {
            if (gameState?.playerSpawnLocation == null)
            {
                Debug.Log("Player spawning disabled");
                return;
            }

            if (!string.IsNullOrEmpty(gameState.playerSpawnLocation))
            {
                var spawns = GameObject.FindGameObjectsWithTag(settings.SpawnTag);
                var foundSpawn = false;

                foreach (GameObject spawn in spawns)
                {
                    if (spawn.name == gameState.playerSpawnLocation)
                    {
                        foundSpawn = true;
                        //gameState.activePlayer = container.Instantiate(settings.PlayerPrefab, spawn.transform.position, Quaternion.identity);
                        gameState.activePlayer = Object.Instantiate(settings.PlayerPrefab, spawn.transform.position, Quaternion.identity);
                        break;
                    }
                }

                if (!foundSpawn)
                {
                    throw new MissingReferenceException($"Could not find the player spawn location with the name {gameState.playerSpawnLocation}");
                }
            }
            else
            {
                //gameState.activePlayer = container.Instantiate(settings.PlayerPrefab, defaultSpawnTransform.position, Quaternion.identity);
                gameState.activePlayer = Object.Instantiate(settings.PlayerPrefab, defaultSpawnTransform.position, Quaternion.identity);
                Debug.Log($"Player spawned at default location: {defaultSpawnTransform}");
            }

            if (gameState.activePlayer)
            {
                GameEvents.Player.onPlayerSpawned.Invoke(gameState.activePlayer.transform);
            }
            else
            {
                throw new MissingReferenceException("No ActivePlayer in PlayerManager. May have failed to spawn!");
            }
        }
    }
}

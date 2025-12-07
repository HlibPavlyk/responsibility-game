using Core.Abstractions;
using Core.DI;
using Core.Events;
using Systems.Game;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Features.Characters.Player
{
    public class PlayerManager : IPlayerManager
    {
        [Inject] private readonly PlayerManagerSettings _settings;
        [Inject] private readonly GameState _gameState;
        private readonly IObjectResolver _container; // Reference to the DI Container
        
        public PlayerManager(IObjectResolver container)
        {
            _container = container;
        }
        
        public void SpawnPlayer(Transform defaultSpawnTransform)
        {
            if (_gameState?.playerSpawnLocation == null)
            {
                Debug.Log("Player spawning disabled");
                return;
            }

            if (!string.IsNullOrEmpty(_gameState.playerSpawnLocation))
            {
                var spawns = GameObject.FindGameObjectsWithTag(_settings.spawnTag);
                var foundSpawn = false;

                foreach (GameObject spawn in spawns)
                {
                    if (spawn.name == _gameState.playerSpawnLocation)
                    {
                        foundSpawn = true;
                        _gameState.activePlayer = Object.Instantiate(_settings.playerPrefab, spawn.transform.position, Quaternion.identity);
                        _container.InjectGameObject(_gameState.activePlayer);
                        break;
                    }
                }

                if (!foundSpawn)
                {
                    throw new MissingReferenceException($"Could not find the player spawn location with the name {_gameState.playerSpawnLocation}");
                }
            }
            else
            {
                _gameState.activePlayer = Object.Instantiate(_settings.playerPrefab, defaultSpawnTransform.position, Quaternion.identity);
                _container.InjectGameObject(_gameState.activePlayer);
                Debug.Log($"Player spawned at default location: {defaultSpawnTransform}");
            }

            if (_gameState.activePlayer)
            {
                GameEvents.Player.OnPlayerSpawned.Invoke(_gameState.activePlayer.transform);
            }
            else
            {
                throw new MissingReferenceException("No ActivePlayer in PlayerManager. May have failed to spawn!");
            }
        }
    }
}

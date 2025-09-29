using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerManager", menuName = "ScriptableObjects/Manager/PlayerManager", order = 1)]
public class PlayerManager : ScriptableObject
{
    [SerializeField]
    private GameObject playerPrefab;
    public GameObject ActivePlayer { get; private set; }


    [SerializeField]
    private PlayerStats startingPlayerStats;
    public PlayerStats PlayerStats;
    public GameState GameState {get; set;}

    public string spawnTag;

    private void OnEnable()
    {
       // LevelEvents.levelLoaded += SpawnPlayer;
    }

    /*protected void SpawnPlayer(Transform defaultSpawnTransform)
    {
        if (GameState.playerSpawnLocation == null)
        {
            Debug.Log("Player spawning disable");
            return;
        }

        if (GameState.playerSpawnLocation != "")
        {
            GameObject[] spawns = GameObject.FindGameObjectsWithTag(spawnTag);
            bool foundSpawn = false;

            foreach (GameObject spawn in spawns)
            {
                if(spawn.name == GameState.playerSpawnLocation)
                {
                    foundSpawn = true;

                    ActivePlayer = Instantiate(playerPrefab, spawn.transform.position, Quaternion.identity);
                    break;
                }
            }
            if(!foundSpawn)
            {
                throw new MissingReferenceException("Could not find the player spawn location with  the name " + GameState.playerSpawnLocation);
            }
        }
        else
        {
            ActivePlayer = Instantiate(playerPrefab, defaultSpawnTransform.position, Quaternion.identity);
            Debug.Log("Player spawned at default location: " + defaultSpawnTransform);
        }

        if (ActivePlayer)
        {
            PlayerEvents.onPlayerSpawned.Invoke(ActivePlayer.transform);
        }
        else
        {
            throw new MissingReferenceException("No ActivePlayer in PlayerManager. May have failed to spawn!");
        }
    }*/

    private void OnDisable()
    {
      //  LevelEvents.levelLoaded -= SpawnPlayer;
    }
}

using UnityEngine;

namespace Core.Abstractions
{
    public interface IPlayerManager
    {
        /*GameObject ActivePlayer { get; set; }
        PlayerStats PlayerStats { get; set; }
        GameState GameState { get; set; }*/
        
        //void Initialize(PlayerStats startingStats, GameObject playerPrefab, string spawnTag);
        void SpawnPlayer(Transform defaultSpawnTransform);
    }
}

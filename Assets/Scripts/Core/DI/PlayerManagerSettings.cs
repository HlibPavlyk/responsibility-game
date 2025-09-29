using System;
using UnityEngine;

namespace Core.DI
{
    [Serializable]
    public class PlayerManagerSettings
    {
        public PlayerStats StartingPlayerStats;
        public GameObject PlayerPrefab;
        public string SpawnTag = "Respawn";
    }
}
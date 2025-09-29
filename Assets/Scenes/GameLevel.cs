using System.Collections;
using System.Collections.Generic;
using Core.Events;
using UnityEngine;

public class GameLevel : MonoBehaviour
{
    public Transform defaultPlayerSpawn;

    // Start the game level
    void Start()
    {
        GameEvents.Level.levelLoaded.Invoke(defaultPlayerSpawn);
    }

}

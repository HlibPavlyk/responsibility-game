using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

[CreateAssetMenu(fileName = "LevelManager", menuName = "ScriptableObjects/Manager/LevelManager", order = 1)]

public class LevelManager : ScriptableObject
{
    //public GameState GameState { get; set; }
    [NonSerialized]
    public bool isTransitionAnimationPlaying = false;  
    
    [Inject] private GameState gameState;

    /*private void OnEnable()
    {
        LevelEvents.levelExit += OnLevelExit;
    }*/
    private void OnLevelExit(string nextLevelSceneName, string playerSpawnTransformName)
    {
        /*//GameState.playerSpawnLocation = playerSpawnTransformName;
        gameState.playerSpawnLocation = playerSpawnTransformName;
        SceneManager.LoadScene(nextLevelSceneName, LoadSceneMode.Single);*/
        
        gameState.playerSpawnLocation = "GameStartSpawn";
        SceneManager.LoadScene("Hall", LoadSceneMode.Single);
    }

    /*private void OnDisable()
    {
        LevelEvents.levelExit -= OnLevelExit;
    }*/
}

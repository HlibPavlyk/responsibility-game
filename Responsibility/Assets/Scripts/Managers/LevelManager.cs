using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "LevelManager", menuName = "ScriptableObjects/Manager/LevelManager", order = 1)]

public class LevelManager : ScriptableObject
{
    public GameState GameState { get; set; }
    [NonSerialized]
    public bool isTransitionAnimationPlaying = false;  

    private void OnEnable()
    {
        LevelEvents.levelExit += OnLevelExit;
    }
    private void OnLevelExit(SceneAsset nextLevel, string playerSpawnTransformName)
    {
        GameState.playerSpawnLocation = playerSpawnTransformName;
        SceneManager.LoadScene(nextLevel.name, LoadSceneMode.Single);
    }

    private void OnDisable()
    {
        LevelEvents.levelExit -= OnLevelExit;
    }
}

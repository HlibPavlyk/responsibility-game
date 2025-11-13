using System;
using System.Collections;
using System.Collections.Generic;
using Core.Abstractions;
using Core.Events;
using UnityEditor;
using UnityEngine;
using VContainer;

public class LevelTransition : SceneTransition
{
    [Header("VisualCue")]
    [SerializeField] private GameObject visualCue;
    private bool playerInRange;
    public string triggereTag = "Player";
    
    
    
    [Inject] private IInputManager inputManager;
    [Inject] private ISfxManager sfxManager; 
    
    protected override void Start()
    {
        if (sceneToLoad == null)
        {
            throw new MissingReferenceException(name + "has no sceneToLoad set");
        }
        playerInRange = false;
        visualCue.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (playerInRange)
        {
            visualCue.SetActive(true);
            if (inputManager.GetInteractPressed())
            {
                StartCoroutine(LoadLevel());
            }
        }
        else
        {
            visualCue.SetActive(false);
        }
    }

    public override IEnumerator LoadLevel(string OtherSceneToLoad = null)
    {
        // door open SFX
        sfxManager?.Play("door_open"); // id from SfxLibrarySettings
        
        gameState.isTransitionAnimationPlaying = true;
        transitionAnimator.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        //test save-upload system
        gameState.playerStats.currentSceneName = sceneToLoad;
        saveLoadManager.SaveGame();

        GameEvents.Level.LevelExit.Invoke(sceneToLoad, playerSpawnTransformName);
        gameState.isTransitionAnimationPlaying = false;
        //GameManager.Instance.LevelManager.isTransitionAnimationPlaying = false;

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == triggereTag)
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == triggereTag)
        {
            playerInRange = false;
        }
    }
}


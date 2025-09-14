using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelTransition : SceneTransition
{
    [Header("VisualCue")]
    [SerializeField] private GameObject visualCue;
    private bool playerInRange;
    public string triggereTag = "Player";

    protected override void Start()
    {
        if (sceneToLoad == null)
        {
            throw new MissingReferenceException(name + "has no sceneToLoad set");
        }
        playerInRange = false;
        visualCue.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange)
        {
            visualCue.SetActive(true);
            if (GameManager.Instance.InputManager.GetInteractPressed())
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
        GameManager.Instance.LevelManager.isTransitionAnimationPlaying = true;
        transitionAnimator.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        //test save-upload system
        GameManager.Instance.PlayerManager.PlayerStats.currentSceneName = sceneToLoad;
        SaveLoadManager.SaveGame();

        LevelEvents.levelExit.Invoke(sceneToLoad, playerSpawnTransformName);
        GameManager.Instance.LevelManager.isTransitionAnimationPlaying = false;

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


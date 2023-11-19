using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelTransition : MonoBehaviour
{
    [Header("VisualCue")]
    [SerializeField] private GameObject visualCue;
    private bool playerInRange;
    public string triggereTag = "Player";
    public string playerSpawnTransformName = "NOT SET";

    public SceneAsset sceneToLoad;

    [SerializeField]
    private Animator transitionAnimator;
    public float transitionTime = 0.5f;

    private void Start()
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

    IEnumerator LoadLevel()
    {
        GameManager.Instance.LevelManager.isTransitionAnimationPlaying = true;
        transitionAnimator.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

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


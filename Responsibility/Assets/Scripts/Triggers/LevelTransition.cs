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
    //public float enterSpeed = 1f;
    public SceneAsset sceneToLoad;
    /* public GameObject fadeAnimation;
     private Canvas canvas;
     private Animator transitionAnimator;*/

    private void Start()
    {
        if (sceneToLoad == null)
        {
            throw new MissingReferenceException(name + "has no sceneToLoad set");
        }
        playerInRange = false;
        visualCue.SetActive(false);
        // transistion animation
    }

    private void Update()
    {
        // transistion animation

        //        if (playerInRange && DialogueManager.GetInstance() != null && !DialogueManager.GetInstance().isDialoguePlaying)
        if (playerInRange /*&& !GameManager.Instance.DialogueManager.isDialoguePlaying*/)

        {
            visualCue.SetActive(true);
            if (GameManager.Instance.InputManager.GetInteractPressed())
            {
                LevelEvents.levelExit.Invoke(sceneToLoad, playerSpawnTransformName);
            }
        }
        else
        {
            visualCue.SetActive(false);
        }
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


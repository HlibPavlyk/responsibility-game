using System.Collections;
using System.Collections.Generic;
using Core.DI;
using ResponsibilityGame.Core.Interfaces;
using Systems.Game;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

[InjectableMonoBehaviour]
public class DialogueTrigger : MonoBehaviour
{
    [Header("VisualCue")]
    [SerializeField] private GameObject visualCue;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;
    
    [Inject] private readonly IDialogueManager dialogueManager;
    [Inject] private readonly IInputManager inputManager;
    [Inject] private readonly GameState gameState;
    
    private bool playerInRange;
    private void Awake()
    {
        playerInRange = false;
        visualCue.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && !gameState.isDialoguePlaying)

        {
            visualCue.SetActive(true);
            if (inputManager.GetInteractPressed())
            {
                dialogueManager.EnterDialogueMode(inkJSON);
            }
        }
        else
        {
            visualCue.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}

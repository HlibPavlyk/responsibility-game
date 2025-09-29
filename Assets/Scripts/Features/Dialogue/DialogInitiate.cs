using System.Collections;
using System.Collections.Generic;
using Core.DI;
using ResponsibilityGame.Core.Interfaces;
using UnityEngine;
using VContainer;

[InjectableMonoBehaviour]
public class DialogInitiate : MonoBehaviour
{
    [Inject] private readonly IDialogueManager dialogueManager;
    
    void Start()
    {
        dialogueManager.Initialize(gameObject);
    }

    void Update()
    {
        dialogueManager.DialogUpdate();
    }
}

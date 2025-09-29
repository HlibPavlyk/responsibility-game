using System.Collections;
using System.Collections.Generic;
using Core.DI;
using ResponsibilityGame.Core.Interfaces;
using UnityEngine;
using VContainer;

public class DialogInitiate : InjectableMonoBehaviour
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

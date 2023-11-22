using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogInitiate : MonoBehaviour
{
    
    void Start()
    {
        GameManager.Instance.DialogueManager.InitiateDialogueMenu(gameObject);
    }

    void Update()
    {
        GameManager.Instance.DialogueManager.DialogUpdate();
    }
}

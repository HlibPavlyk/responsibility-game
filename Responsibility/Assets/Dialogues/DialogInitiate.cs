using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogInitiate : MonoBehaviour
{
    
    void Start()
    {
        DialogEvents.onDialogSpawned.Invoke(gameObject);
    }

    void Update()
    {
        DialogEvents.onDioalogUpdate.Invoke();
    }
}

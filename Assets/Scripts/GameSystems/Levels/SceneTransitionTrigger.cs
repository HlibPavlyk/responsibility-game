using System.Collections;
using System.Collections.Generic;
using Core.Events;
using Core.Interfaces;
using ResponsibilityGame.Core.Interfaces;
using UnityEditor;
using UnityEngine;
using VContainer;

public class SceneTransition : MonoBehaviour
{
    public string playerSpawnTransformName = "NOT SET";
    public string sceneToLoad;

    [SerializeField]
    protected Animator transitionAnimator;
    public float transitionTime = 0.5f;
    
   
    [Inject] protected GameState gameState;
    [Inject] protected ISaveLoadManager saveLoadManager;

    protected virtual void Start()
    {
        if (sceneToLoad == null)
        {
            throw new MissingReferenceException(name + "has no sceneToLoad set");
        }
    }


    public virtual IEnumerator LoadLevel(string OtherSceneToLoad = null)
    {
        transitionAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);

        //test save-upload system
        gameState.playerStats.currentSceneName = sceneToLoad;
        saveLoadManager.SaveGame();

        string sceneChoice;
        if (OtherSceneToLoad != null)
        {
            sceneChoice = OtherSceneToLoad;
        }
        else
        {
            sceneChoice = sceneToLoad;
        }
        GameEvents.Level.levelExit.Invoke(sceneChoice, playerSpawnTransformName);
    }

   
}

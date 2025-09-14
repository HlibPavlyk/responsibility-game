using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    public string playerSpawnTransformName = "NOT SET";
    public string sceneToLoad;

    [SerializeField]
    protected Animator transitionAnimator;
    public float transitionTime = 0.5f;

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
        GameManager.Instance.PlayerManager.PlayerStats.currentSceneName = sceneToLoad;
        SaveLoadManager.SaveGame();

        string sceneChoice;
        if (OtherSceneToLoad != null)
        {
            sceneChoice = OtherSceneToLoad;
        }
        else
        {
            sceneChoice = sceneToLoad;
        }
        LevelEvents.levelExit.Invoke(sceneChoice, playerSpawnTransformName);
    }

   
}

using System;
using System.Collections;
using Core.Events;
using UnityEngine;

namespace Features.Story.Actions
{
    [Serializable]
    public class TransitionSceneAction : StoryAction
    {
        [Header("Scene")]
        [SerializeField] private string sceneName;
        [SerializeField] private string spawnPointName;
        
        [Header("Transition Animation")]
        [SerializeField] private Animator transitionAnimator;
        [SerializeField] private float transitionTime = 0.5f;
        
        [Header("SFX")]
        [SerializeField] private string sfxTag = "door_open";

        public override void Execute(StoryActionContext context)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning($"TransitionSceneAction: sceneName is empty for trigger {context.TriggerID}");
                return;
            }

            if (context.Runner)
            {
                context.Runner.StartCoroutine(LoadLevel(context));
            }
        }
        
        private IEnumerator LoadLevel(StoryActionContext context)
        {
            // door open SFX
            context.SfxManager?.Play(sfxTag); // id from SfxLibrarySettings
        
            context.GameState.isTransitionAnimationPlaying = true;
            transitionAnimator.SetTrigger("Start");

             yield return new WaitForSeconds(transitionTime);

            GameEvents.Level.LevelExit.Invoke(sceneName, spawnPointName);
            context.GameState.isTransitionAnimationPlaying = false;
        }
    }
}

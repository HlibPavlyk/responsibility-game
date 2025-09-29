using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.DI
{
    public class SceneLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder) { }
        
        protected override void Awake()
        {
            base.Awake();
            RegisterAllComponentsInHierarchy<SceneTransition>();
            RegisterAllComponentsInHierarchy<DialogueTrigger>();
        }
        
        private void RegisterAllComponentsInHierarchy<T>() where T : MonoBehaviour
        {
            foreach (var trigger in FindObjectsByType<T>(FindObjectsSortMode.None))
            {
                Container.Inject(trigger);
            }
        }
    }
}
using System;
using UnityEngine;
using VContainer.Unity;

namespace Core.DI
{
    public class SceneLifetimeScope : LifetimeScope
    {
        protected override void Awake()
        {
            base.Awake();
            RegisterAllMonoComponentsInHierarchy();
        }
        
        private void RegisterAllMonoComponentsInHierarchy()
        {
            foreach (var component in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
            {
                var type = component.GetType();
                if (Attribute.IsDefined(type, typeof(InjectableMonoBehaviourAttribute)))
                {
                    Container.Inject(component);
                }
            }
        }
    }
}
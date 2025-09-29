using System.Linq;
using UnityEngine;

namespace Core.DI
{
    /// <summary>
    /// Base class for MonoBehaviours that are created dynamically at runtime 
    /// (via Instantiate, Addressable, etc.) and need dependency injection 
    /// from the current SceneLifetimeScope.
    /// </summary>
    public abstract class InjectableDynamicMonoBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
            var container = FindObjectsByType<SceneLifetimeScope>(FindObjectsSortMode.None).FirstOrDefault()?.Container;
            if (container is null)
            {
                Debug.LogError("SceneLifetimeScope Container not found");
                return;
            }
            
            container.Inject(this);
        }
    }
}
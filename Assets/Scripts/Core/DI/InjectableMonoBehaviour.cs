using System.Linq;
using UnityEngine;

namespace Core.DI
{
    public abstract class InjectableMonoBehaviour : MonoBehaviour
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
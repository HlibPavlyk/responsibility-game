using Core.Abstractions;
using Core.Events;
using UnityEngine;
using VContainer;

namespace Features.Bootstrap
{
    public class BootstrapBehaviour : MonoBehaviour
    {
        [SerializeField] private string initialSceneName;
        
        [Inject] private readonly IPlayerManager _playerManager;
        [Inject] private readonly ILevelManager _levelManager;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            if (string.IsNullOrEmpty(initialSceneName))
            {
                Debug.LogError("Initial scene name is not set");
                return;
            }
            
            GameEvents.Level.LevelExit.Invoke(initialSceneName, "");
        }
        
        private void OnEnable()
        {
            // IPlayerManager events
            GameEvents.Level.LevelLoaded += _playerManager.SpawnPlayer;
            
            // ILevelManager events
            GameEvents.Level.LevelExit += _levelManager.OnLevelExit;
        }

        private void OnDisable()
        {
            // IPlayerManager events
            GameEvents.Level.LevelLoaded -= _playerManager.SpawnPlayer;
            
            // ILevelManager events
            GameEvents.Level.LevelExit -= _levelManager.OnLevelExit;
        }
    }
  
}
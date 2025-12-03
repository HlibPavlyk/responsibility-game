using Core.Abstractions;
using Core.Abstractions.Menu;
using Core.Events;
using Systems.Game;
using UnityEngine;
using VContainer;

namespace Features.Bootstrap
{
    public class BootstrapBehaviour : MonoBehaviour
    {
        [SerializeField] private string initialSceneName;
        
        [Inject] private readonly IPlayerManager _playerManager;
        [Inject] private readonly IOptionsManager _optionsManager;
        [Inject] private readonly ILevelManager _levelManager;
        [Inject] private readonly GameState _gameState;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            // reset game state that blocks input
            _gameState.isPaused = false;
            _gameState.isDialoguePlaying = false;
            _gameState.isTransitionAnimationPlaying = false;
            
            // load default settings
            _optionsManager.LoadSettings();
            
            // load initial scene
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
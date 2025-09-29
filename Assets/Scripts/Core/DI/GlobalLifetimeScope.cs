using Content.Characters.Player;
using Core.Interfaces;
using GameSystems.Player;
using GameSystems.SaveLoad;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using ResponsibilityGame.Core.Interfaces;
using ResponsibilityGame.GameSystems.Game;
using ResponsibilityGame.GameSystems.Dialogue;
using ResponsibilityGame.GameSystems.Menu;
using ResponsibilityGame.GameSystems.Input;
using ResponsibilityGame.GameSystems.Levels;

namespace ResponsibilityGame.Core.DI
{
    public class GlobalLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameSettings settings;
        [SerializeField] private GameState state;
        
        [SerializeField] private MenuManager menuManager;
        [SerializeField] private LevelManager levelManager;
        
        protected override void Configure(IContainerBuilder builder)
        {
            // Register game state from settings
            state.playerStats = settings.PlayerManagerSettings.StartingPlayerStats;
            
            // Register MonoBehaviour components
            builder.RegisterComponentInHierarchy<PlayerSpawner>();
            builder.RegisterComponentInHierarchy<LevelManagerBehaviour>();
            builder.RegisterComponentInHierarchy<MenuManagerBehaviour>();
            builder.RegisterComponentInHierarchy<GameManager>();
            
            // Register scriptable objects
            builder.RegisterInstance(menuManager);
            builder.RegisterInstance(levelManager);
            builder.RegisterInstance(settings.PlayerManagerSettings);
            builder.RegisterInstance(settings.MenuManagerSettings);
            builder.RegisterInstance(settings.DialogueManagerSettings);
            builder.RegisterInstance(state);
            
            // Register core managers
            builder.Register<IPlayerManager, PlayerManagerService>(Lifetime.Scoped);
            builder.Register<ILevelManager, LevelManagerService>(Lifetime.Scoped);
            builder.Register<IMenuManager, MenuManagerService>(Lifetime.Scoped);
            builder.Register<ISaveLoadManager, SaveLoadManagerService>(Lifetime.Scoped);
            builder.Register<IDialogueManager, DialogueManagerService>(Lifetime.Scoped);
            builder.Register<IInputManager, InputManagerService>(Lifetime.Singleton);
        }
        
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }
}

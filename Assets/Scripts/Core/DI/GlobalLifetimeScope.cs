using Content.Characters.Player;
using Core.Interfaces;
using GameSystems.Player;
using GameSystems.SaveLoad;
using ResponsibilityGame.Core.DI;
using ResponsibilityGame.Core.Interfaces;
using ResponsibilityGame.GameSystems.Dialogue;
using ResponsibilityGame.GameSystems.Input;
using ResponsibilityGame.GameSystems.Levels;
using ResponsibilityGame.GameSystems.Menu;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.DI
{
    public class GlobalLifetimeScope : LifetimeScope
    {
        [SerializeField] private GameSettings settings;
        [SerializeField] private GameState state;
        
        protected override void Configure(IContainerBuilder builder)
        {
            // Register game state from settings
            state.playerStats = settings.PlayerManagerSettings.StartingPlayerStats;
            
            // Register MonoBehaviour components
            builder.RegisterComponentInHierarchy<PlayerSpawner>();
            builder.RegisterComponentInHierarchy<LevelManagerBehaviour>();
            builder.RegisterComponentInHierarchy<MenuManagerBehaviour>();
            
            // Register scriptable objects
            builder.RegisterInstance(settings.PlayerManagerSettings);
            builder.RegisterInstance(settings.MenuManagerSettings);
            builder.RegisterInstance(settings.DialogueManagerSettings);
            builder.RegisterInstance(state);
            
            // Register core managers
            builder.Register<IPlayerManager, PlayerManager>(Lifetime.Scoped);
            builder.Register<ILevelManager, LevelManager>(Lifetime.Scoped);
            builder.Register<IMenuManager, MenuManager>(Lifetime.Scoped);
            builder.Register<ISaveLoadManager, SaveLoadManager>(Lifetime.Scoped);
            builder.Register<IDialogueManager, DialogueManager>(Lifetime.Scoped);
            builder.Register<IInputManager, InputManager>(Lifetime.Singleton);
        }
        
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }
}

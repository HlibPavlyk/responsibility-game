using Core.Abstractions;
using Features.Bootstrap;
using Features.Characters.Player;
using ResponsibilityGame.GameSystems.Dialogue;
using ResponsibilityGame.GameSystems.Input;
using ResponsibilityGame.GameSystems.Levels;
using ResponsibilityGame.GameSystems.Menu;
using Systems.Game;
using Systems.SaveLoad;
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
            // Register MonoBehaviour components
            builder.RegisterComponentInHierarchy<BootstrapBehaviour>();
            
            // Register scriptable objects
            builder.RegisterInstance(settings.playerManagerSettings);
            builder.RegisterInstance(settings.menuManagerSettings);
            builder.RegisterInstance(settings.dialogueManagerSettings);
            builder.RegisterInstance(state);
            
            // Register core managers
            builder.Register<IPlayerManager, PlayerManager>(Lifetime.Scoped);
            builder.Register<ILevelManager, LevelManager>(Lifetime.Scoped);
            builder.Register<IMenuManager, MenuManager>(Lifetime.Scoped);
            builder.Register<IDialogueManager, DialogueManager>(Lifetime.Scoped);
            builder.Register<IInputManager, InputManager>(Lifetime.Singleton);
            
            #if UNITY_WEBGL && !UNITY_EDITOR
                builder.Register<ISaveLoadManager, WebSaveLoadManager>(Lifetime.Scoped);
            #else
                builder.Register<ISaveLoadManager, SaveLoadManager>(Lifetime.Scoped);
            #endif
        }
        
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }
}

using Core.Abstractions;
using Core.Abstractions.Menu;
using Features.Bootstrap;
using Features.Characters.Player;
using Features.Levels;
using Features.Menu.MainMenu;
using Features.Menu.Options;
using Features.Menu.PauseMenu;
using ResponsibilityGame.GameSystems.Dialogue;
using Systems.Game;
using Systems.Input;
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
            builder.RegisterInstance(settings);
            builder.RegisterInstance(settings.playerManagerSettings);
            builder.RegisterInstance(settings.menuManagerSettings);
            builder.RegisterInstance(settings.dialogueManagerSettings);
            builder.RegisterInstance(settings.audioManagerSettings);
            builder.RegisterInstance(settings.generalSettings);
            builder.RegisterInstance(state);

            // Register core managers
            builder.Register<IPlayerManager, PlayerManager>(Lifetime.Scoped);
            builder.Register<ILevelManager, LevelManager>(Lifetime.Scoped);
            builder.Register<IMenuManager, MenuManager>(Lifetime.Scoped);
            builder.Register<IPauseMenuManager, PauseMenuManager>(Lifetime.Singleton);
            builder.Register<IDialogueManager, DialogueManager>(Lifetime.Scoped);
            builder.Register<IInputManager, InputManager>(Lifetime.Singleton);
            builder.Register<IOptionsManager, OptionsManager>(Lifetime.Singleton);
            
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

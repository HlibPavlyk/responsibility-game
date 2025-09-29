using Core.Interfaces;
using UnityEngine;
using VContainer;
using ResponsibilityGame.Core.Interfaces;

namespace ResponsibilityGame.GameSystems.Game
{
    public class GameManagerService : IGameManager
    {
        public GameState GameState { get; private set; }
        public ILevelManager LevelManager { get; private set; }
        public IPlayerManager PlayerManager { get; private set; }
        public IDialogueManager DialogueManager { get; private set; }
        public IInputManager InputManager { get; private set; }
        public IMenuManager MenuManager { get; private set; }

        [Inject]
        public GameManagerService(
            GameState startingGameState,
            ILevelManager levelManager,
            IPlayerManager playerManager,
            IDialogueManager dialogueManager,
            IInputManager inputManager,
            IMenuManager menuManager)
        {
            GameState = Object.Instantiate(startingGameState);
            LevelManager = levelManager;
            PlayerManager = playerManager;
            DialogueManager = dialogueManager;
            InputManager = inputManager;
            MenuManager = menuManager;
        }

        public void Initialize()
        {
            // Set GameState references for managers that need it
            //LevelManager.GameState = GameState;
            //PlayerManager.GameState = GameState;
            
            Debug.Log("GameManagerService initialized successfully");
        }

        public void SetGameState(GameState gameState)
        {
            //GameState = gameState;
            //LevelManager.GameState = gameState;
            //PlayerManager.GameState = gameState;
        }
    }
}

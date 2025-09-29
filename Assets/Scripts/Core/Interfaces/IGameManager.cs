using Core.Interfaces;
using UnityEngine;

namespace ResponsibilityGame.Core.Interfaces
{
    public interface IGameManager
    {
        GameState GameState { get; }
        ILevelManager LevelManager { get; }
        IPlayerManager PlayerManager { get; }
        IDialogueManager DialogueManager { get; }
        IInputManager InputManager { get; }
        IMenuManager MenuManager { get; }
        
        void Initialize();
        void SetGameState(GameState gameState);
    }
}

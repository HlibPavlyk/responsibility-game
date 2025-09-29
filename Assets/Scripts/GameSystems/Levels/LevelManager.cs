using UnityEngine;
using UnityEngine.SceneManagement;
using ResponsibilityGame.Core.Interfaces;
using VContainer;

namespace ResponsibilityGame.GameSystems.Levels
{
    public class LevelManager : ILevelManager
    {
        public GameState GameState { get; set; }
        public bool IsTransitionAnimationPlaying { get; set; }

        [Inject] private GameState gameState;

        public void OnLevelExit(string nextSceneName, string playerSpawnTransformName)
        {
            if (gameState)
            {
                gameState.playerSpawnLocation = playerSpawnTransformName;
            }
            SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
        }
    }
}

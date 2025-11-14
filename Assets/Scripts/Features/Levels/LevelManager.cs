using Core.Abstractions;
using Systems.Game;
using UnityEngine.SceneManagement;
using VContainer;

namespace Features.Levels
{
    public class LevelManager : ILevelManager
    {
        [Inject] private GameState _gameState;

        public void OnLevelExit(string nextSceneName, string playerSpawnTransformName)
        {
            if (_gameState)
            {
                _gameState.playerSpawnLocation = playerSpawnTransformName;
            }
            SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
        }
    }
}

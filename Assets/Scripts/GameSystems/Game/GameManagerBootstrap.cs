using Core.Interfaces;
using GameSystems.Player;
using UnityEngine;
using VContainer;
using ResponsibilityGame.Core.Interfaces;

namespace ResponsibilityGame.GameSystems.Game
{
    public class GameManagerBootstrap : MonoBehaviour
    {
        [Inject] private IGameManager gameManager;
        [Inject] private IDialogueManager dialogueManager;
        [Inject] private IMenuManager menuManager;
        [Inject] private IPlayerManager playerManager;

        private void Start()
        {
            // Initialize all managers
            gameManager.Initialize();
            
            // Initialize dialogue and menu systems with canvas
            GameObject canvas = FindObjectOfType<Canvas>()?.gameObject;
            if (canvas != null)
            {
                dialogueManager.Initialize(canvas);
               // menuManager.Initialize("", canvas);
            }
            
            //todo
            /*// Initialize player manager
            if (playerManager is PlayerManagerService playerService)
            {
                playerService.Initialize(playerManager.PlayerStats, null, playerManager.SpawnTag);
            }*/
            
            Debug.Log("Game systems initialized successfully with VContainer DI");
        }

        private void Update()
        {
            // Update dialogue system
            dialogueManager?.DialogUpdate();
        }

        private void OnDestroy()
        {
            // Cleanup managers
            dialogueManager?.Cleanup();
            
            if (playerManager is PlayerManagerService playerService)
            {
               // playerService.Cleanup();
            }
        }
    }
}

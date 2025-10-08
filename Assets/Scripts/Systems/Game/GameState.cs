using UnityEngine;

namespace Systems.Game
{
    [CreateAssetMenu(fileName = "GameState", menuName = "ScriptableObjects/GameState", order =1 )]
    public class GameState : ScriptableObject
    {
        //Implement encapsulation
        public string playerSpawnLocation = "";
        public GameObject activePlayer;
        public PlayerStats playerStats;
    
        //transition animation
        public bool isTransitionAnimationPlaying;
    
        // dialogue manager
        public bool isDialoguePlaying;
        
        // pause menu manager
        public bool isPaused ;
    }
}

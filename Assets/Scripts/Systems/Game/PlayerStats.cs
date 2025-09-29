using UnityEngine;

namespace Systems.Game
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "ScriptableObjects/PlayerStats", order = 1)]
    public class PlayerStats : ScriptableObject
    {
        //Player Stats and other logic
        public string currentSceneName;
    }
}

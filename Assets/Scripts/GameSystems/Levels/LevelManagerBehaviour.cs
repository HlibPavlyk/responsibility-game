using Core.Events;
using ResponsibilityGame.Core.Interfaces;
using UnityEngine;
using VContainer;

namespace ResponsibilityGame.GameSystems.Levels
{
    public class LevelManagerBehaviour : MonoBehaviour
    {
        [Inject] private ILevelManager levelManager;

        private void OnEnable()
        {
            GameEvents.Level.levelExit += levelManager.OnLevelExit;
        }

        private void OnDisable()
        {
            GameEvents.Level.levelExit -= levelManager.OnLevelExit;
        }
    }

}
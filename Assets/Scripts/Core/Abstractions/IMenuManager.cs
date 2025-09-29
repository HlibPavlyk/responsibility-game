using UnityEngine;

namespace ResponsibilityGame.Core.Interfaces
{
    public interface IMenuManager
    {
        //void Initialize(string startSceneName, GameObject canvas);
        void NewGame();
        void ContinueGame();
        void LoadGame();
        void OpenSettings();
        void ShowAboutInfo();
        void ExitGame();
    }
}

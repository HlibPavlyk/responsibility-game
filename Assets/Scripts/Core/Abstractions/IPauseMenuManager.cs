namespace Core.Abstractions
{
    public interface IPauseMenuManager
    {
        void TogglePause();
        void Resume();
        void OpenOptions();
        void ResetLevel();
        void LoadGame();
        void ReturnToMainMenu();
        void QuitGame();
    }
}

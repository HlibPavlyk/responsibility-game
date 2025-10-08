namespace Core.Abstractions
{
    public interface IPauseMenuManager
    {
        void TogglePause();
        void Resume();
        void OpenOptions();
        void ResetLevel();
        void ReturnToMainMenu();
        void QuitGame();
    }
}

using Features.Menu.Options;

namespace Core.Abstractions.Menu
{
    public interface IPauseMenuManager
    {
        void TogglePause();
        void Resume();
        void OpenOptions(OptionsWindowUI optionsWindowUI);
        void ResetLevel();
        void ReturnToMainMenu();
        void QuitGame();
    }
}

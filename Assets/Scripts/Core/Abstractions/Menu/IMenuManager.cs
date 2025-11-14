using Features.Menu.About;
using Features.Menu.Options;

namespace Core.Abstractions.Menu
{
    public interface IMenuManager
    {
        bool hasSaveGame { get; }
        void NewGame();
        void ContinueGame();
        void LoadGame();
        void OpenSettings(OptionsWindowUI optionsWindowUI);
        void ShowAboutInfo(AboutWindowUI aboutWindowUI);
        void ExitGame();
    }
}

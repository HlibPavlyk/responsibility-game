namespace Core.Abstractions
{
    public interface IMenuManager
    {
        bool hasSaveGame { get; }
        void NewGame();
        void ContinueGame();
        void LoadGame();
        void OpenSettings();
        void ShowAboutInfo();
        void ExitGame();
    }
}

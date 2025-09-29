namespace ResponsibilityGame.Core.Interfaces
{
    public interface ISaveLoadManager
    {
        void SaveGame();
        void LoadGame();
        void DeleteSaves();
        bool HasSaveFile();
    }
}

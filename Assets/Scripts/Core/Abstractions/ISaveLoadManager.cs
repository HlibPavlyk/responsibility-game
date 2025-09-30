namespace Core.Abstractions
{
    public interface ISaveLoadManager
    {
        void SaveGame();
        void LoadGame();
        void DeleteSaves();
        bool HasSaveFile();
    }
}

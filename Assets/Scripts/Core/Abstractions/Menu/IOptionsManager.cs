namespace Core.Abstractions.Menu
{
    public interface IOptionsManager
    {
        // Audio settings
        float musicVolume { get; set; }
        float sfxVolume { get; set; }

        // Dialogue settings
        float dialogueSpeed { get; set; }

        // Language settings
        string language { get; set; }

        // Methods
        void LoadSettings();
        void SaveSettings();
        void ResetToDefaults();
    }
}
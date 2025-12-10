namespace Features.Story
{
    /// <summary>
    /// Centralized constants for common story flags.
    /// Using constants provides compile-time checking and prevents typos.
    /// You can add your own flags here as your story progresses.
    /// </summary>
    public static class StoryFlags
    {
        // Example flags - replace with your actual story flags
        public const string BossMet = "boss_met";
        public const string MinigameCompleted = "minigame_completed";
        public const string ReportSubmitted = "report_submitted";
        public const string BossThanked = "boss_thanked";
        public const string HasKeycard = "has_keycard";

        // Day progression flags
        public const string Day1Started = "day1_started";
        public const string Day1Completed = "day1_completed";
        public const string Day2Started = "day2_started";

        // Tutorial flags
        public const string TutorialCompleted = "tutorial_completed";
        public const string FirstDialogueSeen = "first_dialogue_seen";
    }
}

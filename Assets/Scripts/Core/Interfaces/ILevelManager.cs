namespace ResponsibilityGame.Core.Interfaces
{
    public interface ILevelManager
    {
        // GameState GameState { get; set; }
        //bool IsTransitionAnimationPlaying { get; set; }
        
        void OnLevelExit(string nextSceneName, string playerSpawnTransformName);
    }
}

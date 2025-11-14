namespace Core.Abstractions.Menu
{
    public interface IMenuInteraction
    {
        void EnableInteraction(bool enable);
        void SelectOptionsButton();
    }
}
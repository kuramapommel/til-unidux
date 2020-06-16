namespace Pommel.Reversi.Domain.Transition
{
    public interface IScene
    {
        string Id { get; }

        bool IsBase { get; }
    }
}
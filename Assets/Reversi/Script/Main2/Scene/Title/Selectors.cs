namespace Pommel.Reversi.Scene.Title
{
    public interface ISelector
    {
        int Count { get; }
    }

    public sealed class Selector : ISelector
    {
        private readonly IState m_state;

        public int Count => m_state.Count;

        public Selector(IState state) => m_state = state;
    }
}
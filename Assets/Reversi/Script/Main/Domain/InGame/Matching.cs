namespace Pommel.Reversi.Domain.InGame
{
    public interface IMatching
    {
        string Id { get; }

        IPlayer FirstPlayer { get; }

        IPlayer SecondPlayer { get; }
    }

    public interface IMatchingFactory
    {
        IMatching Create(string id, IPlayer first = default, IPlayer second = default);
    }

    public sealed class Matching : IMatching
    {
        public string Id { get; }

        public IPlayer FirstPlayer { get; }

        public IPlayer SecondPlayer { get; }

        public Matching(string id, IPlayer first = default, IPlayer second = default)
        {
            Id = id;
            FirstPlayer = first ?? Player.None;
            SecondPlayer = second ?? Player.None;
        }
    }
}
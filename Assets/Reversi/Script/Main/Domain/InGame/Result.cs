
namespace Pommel.Reversi.Domain.InGame
{
    public interface IGameResult
    {
        string Id { get; }

        (int white, int black) Count { get; }

        Winner Winner { get; }
    }

    public sealed class GameResult : IGameResult
    {
        public string Id { get; }

        public (int white, int black) Count { get; }

        public Winner Winner { get; }
    }

    public enum Winner
    {
        Undecided,
        White,
        Black,
        Draw
    }
}
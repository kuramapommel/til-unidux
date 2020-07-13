
namespace Pommel.Server.Domain.InGame
{
    public interface IGameResult
    {
        string Id { get; }

        (int dark, int light) Count { get; }

        Winner Winner { get; }
    }

    public interface IGameResultFactory
    {
        IGameResult Create(string id, int darkCount, int lightCount, Winner winner);
    }

    public sealed class GameResult : IGameResult
    {
        public string Id { get; }

        public (int dark, int light) Count { get; }

        public Winner Winner { get; }

        public GameResult(string id, int darkCount, int lightCount, Winner winner)
        {
            Id = id;
            Count = (darkCount, lightCount);
            Winner = winner;
        }
    }

    public enum Winner
    {
        Undecided,
        White,
        Black,
        Draw
    }
}
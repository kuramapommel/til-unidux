
namespace Pommel.Reversi.Domain.InGame
{
    public interface IGameResult
    {
        string Id { get; }

        (int white, int black) Count { get; }

        Winner Winner { get; }

        // todo 報酬などあるならもたせる
    }

    public enum Winner
    {
        White,
        Black,
        Draw
    }
}
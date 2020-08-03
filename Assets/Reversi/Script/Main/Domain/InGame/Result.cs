namespace Pommel.Reversi.Domain.InGame
{
    public readonly struct GameResult
    {
        public (int dark, int light) Count { get; }

        public Winner Winner { get; }

        public GameResult(int dark, int light, Winner winner)
        {
            Count = (dark, light);
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
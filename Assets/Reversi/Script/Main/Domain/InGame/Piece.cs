namespace Pommel.Reversi.Domain.InGame
{
    public sealed class Piece
    {
        public Point Point { get; }

        public Color Color { get; }

        public Piece(Point point, Color color = Color.None)
        {
            Point = point;
            Color = color;
        }
    }

    public enum Color
    {
        None,
        Dark,
        Light
    }
}
namespace Pommel.Reversi.Domain.InGame
{
    public sealed class Stone
    {
        public Point Point { get; }

        public Color Color { get; }

        public Stone SetColor(Color color) => new Stone(Point, color);

        public Stone(Point point, Color color = Color.None)
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
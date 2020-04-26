namespace Pommel.Reversi.Domain.InGame
{
    public readonly struct Stone
    {
        public Point Point {get;}

        public Color Color {get;}

        public Stone(Point point, Color color = Color.None)
        {
            Point = point;
            Color = color;
        }
    }

    public enum Color
    {
        None,
        Black,
        White
    }
}
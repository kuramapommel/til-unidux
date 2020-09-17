using System;

namespace Pommel.Server.Domain.InGame
{
    public sealed class Piece
    {
        public Point Point { get; }

        public Color Color { get; }

        public Piece SetColor(Color color) => new Piece(Point, color);

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

    public static class ColorExt
    {
        public static int ToInt(this Color color)
        {
            switch (color)
            {
                case Color.None: return 0;
                case Color.Dark: return 1;
                case Color.Light: return 2;
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}
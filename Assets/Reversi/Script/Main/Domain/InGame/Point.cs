using System;

namespace Pommel.Reversi.Domain.InGame
{
    public readonly struct Point : IEquatable<Point>
    {
        public int X { get; }

        public int Y { get; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Point other) => other.X == X && other.Y == Y;

        public override bool Equals(object obj) => obj is Point other && Equals(other);

        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();
    }
}
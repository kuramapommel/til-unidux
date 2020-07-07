using System;
using System.Collections.Generic;
using System.Linq;

namespace Pommel.Server.Domain.InGame
{
    public sealed class Point : IEquatable<Point>
    {
        public int X { get; }

        public int Y { get; }

        private readonly IDictionary<Vector, (int x, int y)> m_adjacents;

        private readonly IDictionary<Vector, (int x, int y)[]> m_radials;

        public Point(int x, int y)
        {
            X = x;
            Y = y;

            m_adjacents = CreateAdjacentVectors(X, Y).ToDictionary(
                vector => vector,
                vector => CreateAdjacentPoints(vector, X, Y));

            m_radials = m_adjacents.ToDictionary(
                element => element.Key,
                element => CreateRadials(element).ToArray());
        }

        public IEnumerable<Point> AdjacentPoints => m_adjacents.Values.Select(adjacent => new Point(adjacent.x, adjacent.y)).ToArray();

        public IEnumerable<Point> CreateSpecifiedVectorPoints(Point point)
        {
            var vector = m_adjacents.First(adjacent => adjacent.Value.x == point.X && adjacent.Value.y == point.Y).Key;
            return m_radials.TryGetValue(vector, out var radials)
                ? radials.Select(radial => new Point(radial.x, radial.y)).ToArray()
                : throw new ArgumentOutOfRangeException();
        }

        public bool Equals(Point other) => other.X == X && other.Y == Y;

        public override bool Equals(object obj) => obj is Point other ? Equals(other) : false;

        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();

        private static (int x, int y) CreateAdjacentPoints(Vector vector, int x, int y)
        {
            switch (vector)
            {
                case Vector.LeftUpper: return (--x, --y);
                case Vector.Upper: return (x, --y);
                case Vector.RightUpper: return (++x, --y);
                case Vector.Left: return (--x, y);
                case Vector.Right: return (++x, y);
                case Vector.LeftLower: return (--x, ++y);
                case Vector.Lower: return (x, ++y);
                case Vector.RightLower: return (++x, ++y);
            }

            throw new ArgumentOutOfRangeException();
        }

        private static IEnumerable<(int x, int y)> CreateRadials(KeyValuePair<Vector, (int x, int y)> element)
        {
            switch (element.Key)
            {
                case Vector.LeftUpper:
                    for (var point = element.Value; point.x >= 0 && point.y >= 0; point = (--point.x, --point.y)) yield return point;
                    yield break;
                case Vector.Upper:
                    for (var point = element.Value; point.y >= 0; point = (point.x, --point.y)) yield return point;
                    yield break;
                case Vector.RightUpper:
                    for (var point = element.Value; point.x <= 7 && point.y >= 0; point = (++point.x, --point.y)) yield return point;
                    yield break;
                case Vector.Left:
                    for (var point = element.Value; point.x >= 0; point = (--point.x, point.y)) yield return point;
                    yield break;
                case Vector.Right:
                    for (var point = element.Value; point.x <= 7; point = (++point.x, point.y)) yield return point;
                    yield break;
                case Vector.LeftLower:
                    for (var point = element.Value; point.x >= 0 && point.y <= 7; point = (--point.x, ++point.y)) yield return point;
                    yield break;
                case Vector.Lower:
                    for (var point = element.Value; point.y <= 7; point = (point.x, ++point.y)) yield return point;
                    yield break;
                case Vector.RightLower:
                    for (var point = element.Value; point.x <= 7 && point.y <= 7; point = (++point.x, ++point.y)) yield return point;
                    yield break;
            }

            throw new ArgumentOutOfRangeException();
        }

        private static IEnumerable<Vector> CreateAdjacentVectors(int x, int y)
        {
            switch ((x, y))
            {
                case var _ when x > 0 && x < 7 && y > 0 && y < 7: // 端以外
                    return new Vector[]
                    {
                            Vector.LeftUpper,
                            Vector.Upper,
                            Vector.RightUpper,
                            Vector.Left,
                            Vector.Right,
                            Vector.LeftLower,
                            Vector.Lower,
                            Vector.RightLower
                    };
                case var _ when x == 0 && y == 0: // 左上の角
                    return new Vector[]
                    {
                            Vector.Right,
                            Vector.Lower,
                            Vector.RightLower
                    };
                case var _ when x == 0 && y == 7: // 左下の角
                    return new Vector[]
                    {
                            Vector.Upper,
                            Vector.RightUpper,
                            Vector.Right
                    };
                case var _ when x == 7 && y == 0: // 右上の角
                    return new Vector[]
                    {
                            Vector.Left,
                            Vector.LeftLower,
                            Vector.Lower
                    };
                case var _ when x == 7 && y == 7: // 右下の角
                    return new Vector[]
                    {
                            Vector.LeftUpper,
                            Vector.Upper,
                            Vector.Left
                    };
                case var _ when x == 0: // 左端
                    return new Vector[]
                    {
                            Vector.Upper,
                            Vector.RightUpper,
                            Vector.Right,
                            Vector.Lower,
                            Vector.RightLower
                    };
                case var _ when y == 0: // 上端
                    return new Vector[]
                    {
                            Vector.Left,
                            Vector.Right,
                            Vector.LeftLower,
                            Vector.Lower,
                            Vector.RightLower
                    };
                case var _ when x == 7: // 右端
                    return new Vector[]
                    {
                            Vector.LeftUpper,
                            Vector.Upper,
                            Vector.Left,
                            Vector.LeftLower,
                            Vector.Lower
                    };
                case var _ when y == 7: // 下端
                    return new Vector[]
                    {
                            Vector.LeftUpper,
                            Vector.Upper,
                            Vector.RightUpper,
                            Vector.Left,
                            Vector.Right
                    };
            }

            throw new ArgumentOutOfRangeException();
        }

        public static IEnumerable<Point> InitialLightPoints => initialLightPoints.Value;

        public static IEnumerable<Point> InitialDarkPoints => initialDarkPoints.Value;

        private static readonly Lazy<IEnumerable<Point>> initialLightPoints = new Lazy<IEnumerable<Point>>(() => new[]
        {
            new Point(3, 3),
            new Point(4, 4)
        });

        private static readonly Lazy<IEnumerable<Point>> initialDarkPoints = new Lazy<IEnumerable<Point>>(() => new[]
        {
            new Point(3, 4),
            new Point(4, 3)
        });
    }

    public enum Vector
    {
        LeftUpper,
        Upper,
        RightUpper,
        Left,
        Right,
        LeftLower,
        Lower,
        RightLower
    }
}
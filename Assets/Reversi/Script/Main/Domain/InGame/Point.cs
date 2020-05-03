using System;
using System.Collections.Generic;
using System.Linq;

namespace Pommel.Reversi.Domain.InGame
{
    public readonly struct Point
    {
        public int X { get; }

        public int Y { get; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public IEnumerable<Point> AdjacentPoints
        {
            get
            {
                // todo なんかキモいからリファクタしたい
                var minX = X - 1 < 0 ? X : X - 1;
                var maxX = X + 1 > 7 ? X : X + 1;
                var minY = Y - 1 < 0 ? Y : Y - 1;
                var maxY = Y + 1 > 7 ? Y : Y + 1;
                return Enumerable.Range(minX, maxX + 1)
                    .SelectMany(x => Enumerable.Range(minY, maxY + 1)
                        .Select(y => new Point(x, y)));
            }
        }

        public IEnumerable<Point> CreateSpecifiedVector(Point point)
        {
            // 隣り合っていないとベクトル指定として不正
            if (!AdjacentPoints.Any(adjacent => adjacent.X == point.X && adjacent.Y == point.Y)) throw new ArgumentException();

            switch (point)
            {
                case var _ when point.X == X && point.Y > Y: // 上に置いたとき
                    return Enumerable.Range(point.Y, 8 - point.Y).Select(y => new Point(point.X, y)).OrderBy(p => p.Y);

                case var _ when point.X == X && point.Y < Y: // 下に置いたとき
                    return Enumerable.Range(0, point.Y + 1).Select(y => new Point(point.X, y)).OrderByDescending(p => p.Y);

                case var _ when point.X > X && point.Y == Y: // 左に置いたとき
                    return Enumerable.Range(point.X, 8 - point.X).Select(x => new Point(x, point.Y)).OrderBy(p => p.X);

                case var _ when point.X < X && point.Y == Y: // 右に置いたとき
                    return Enumerable.Range(0, point.X + 1).Select(x => new Point(x, point.Y)).OrderByDescending(p => p.X);

                case var _ when point.X > X && point.Y > Y: // 左上に置いたとき
                    return Enumerable.Range(point.X, 8 - point.X).SelectMany(x => Enumerable.Range(point.Y, 8 - point.Y).Select(y => new Point(x, y)))
                        .OrderBy(p => p.X).ThenBy(p => p.Y);

                case var _ when point.X > X && point.Y < Y: // 左下に置いたとき
                    return Enumerable.Range(point.X, 8 - point.X).SelectMany(x => Enumerable.Range(0, point.Y + 1).Select(y => new Point(x, y)))
                        .OrderBy(p => p.X).ThenByDescending(p => p.Y);

                case var _ when point.X < X && point.Y > Y: // 右上に置いたとき
                    return Enumerable.Range(0, point.X + 1).SelectMany(x => Enumerable.Range(point.Y, 8 - point.Y).Select(y => new Point(x, y)))
                        .OrderByDescending(p => p.X).ThenBy(p => p.Y);

                case var _ when point.X < X && point.Y < Y: // 右下に置いたとき
                    return Enumerable.Range(0, point.X + 1).SelectMany(x => Enumerable.Range(0, point.Y + 1).Select(y => new Point(x, y)))
                        .OrderByDescending(p => p.X).ThenBy(p => p.Y);
            }

            throw new ArgumentException();
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
}
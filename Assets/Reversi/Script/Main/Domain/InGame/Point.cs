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
                    .SelectMany(x => Enumerable.Range(minY, minY + 1)
                        .Select(y => new Point(x, y)));
            }
        }

        public IEnumerable<Point> CreateSpecifiedVector(Point point)
        {
            // 隣り合っていないとベクトル指定として不正
            if (AdjacentPoints.Any(adjacent => adjacent.X == point.X && adjacent.Y == point.Y)) throw new ArgumentException();

            switch (point)
            {
                // todo なんかキモいからリファクタしたい
                case var _ when point.X == point.X && point.Y > Y: return Enumerable.Range(point.Y, 8 - point.Y).Select(y => new Point(point.X, y));
                case var _ when point.X == point.X && point.Y < Y: return Enumerable.Range(0, point.Y + 1).Select(y => new Point(point.X, y)).Reverse();
                case var _ when point.X > point.X && point.Y == Y: return Enumerable.Range(point.X, 8 - point.X).Select(x => new Point(x, point.Y));
                case var _ when point.X < point.X && point.Y == Y: return Enumerable.Range(0, point.X + 1).Select(x => new Point(x, point.Y)).Reverse();

                case var _ when point.X > point.X && point.Y > Y:
                    return Enumerable.Range(point.X, 8 - point.X).SelectMany(x => Enumerable.Range(point.Y, 8 - point.Y).Select(y => new Point(x, y)));
                case var _ when point.X > point.X && point.Y < Y:
                    return Enumerable.Range(point.X, 8 - point.X).SelectMany(x => Enumerable.Range(0, point.Y + 1).Select(y => new Point(x, y))).Reverse();
                case var _ when point.X < point.X && point.Y > Y:
                    return Enumerable.Range(0, point.X + 1).SelectMany(x => Enumerable.Range(point.Y, 8 - point.Y).Select(y => new Point(x, y))).Reverse();
                case var _ when point.X < point.X && point.Y < Y:
                    return Enumerable.Range(0, point.X + 1).SelectMany(x => Enumerable.Range(0, point.Y + 1).Select(y => new Point(x, y))).Reverse();
            }

            throw new ArgumentException();
        }
    }
}
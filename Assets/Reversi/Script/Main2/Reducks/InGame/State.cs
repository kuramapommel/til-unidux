using System;
using System.Linq;
using Unidux;

namespace Pommel.Reversi.Reducks.InGame
{
    public interface IProps
    {
        ValueObject.Room Room { get; }

        Elements.Board.IProps Board { get; }
    }

    public abstract class InGameState : StateElement
    {
        public abstract ValueObject.Room Room { get; set;  }

        public abstract Elements.Board.State Board { get; }
    }

    public static class State
    {
        private static readonly Lazy<Impl> m_instance = new Lazy<Impl>(() => new Impl());

        public static InGameState Element => m_instance.Value;

        public static IProps Props => m_instance.Value;

        private sealed class Impl : InGameState, IProps
        {
            public override ValueObject.Room Room { get; set; } = new ValueObject.Room(
                string.Empty,
                new ValueObject.Room.Player(
                    string.Empty,
                    string.Empty,
                    ValueObject.Stone.Color.Dark,
                    true
                    ),
                new ValueObject.Room.Player(
                    string.Empty,
                    string.Empty,
                    ValueObject.Stone.Color.Light,
                    false
                    )
                );

            public override Elements.Board.State Board => Elements.Board.Element;

            Elements.Board.IProps IProps.Board => Elements.Board.Props;
        }
    }

    public static class Elements
    {
        public static class Board
        {
            private static readonly Lazy<Impl> m_instance = new Lazy<Impl>(() => new Impl());

            public static State Element => m_instance.Value;

            public static IProps Props => m_instance.Value;

            public interface IProps
            {
                ValueObject.Stone[] Stones { get; }
            }

            public abstract class State : StateElement
            {
                public abstract ValueObject.Stone[] Stones { get; set; }
            }

            private sealed class Impl : State, IProps
            {
                public override ValueObject.Stone[] Stones { get; set; } = Enumerable.Range(0, 8)
                    .SelectMany(x => Enumerable.Range(0, 8)
                    .Select(y => new ValueObject.Stone(new ValueObject.Point(x, y))))
                    .ToArray();
            }
        }
    }

    public static class ValueObject
    {
        public readonly struct Room
        {
            public string RoomId { get; }

            public Player FirstPlayer { get; }

            public Player SecondPlayer { get; }

            public Room(
                string roomId,
                Player firstPlayer,
                Player secondPlayer
                )
            {
                RoomId = roomId;
                FirstPlayer = firstPlayer;
                SecondPlayer = secondPlayer;
            }

            public readonly struct Player
            {
                public string Id { get; }

                public string Name { get; }

                public Stone.Color StoneColor { get; }

                public bool IsTurnPlayer { get; }

                public Player(string id, string name, Stone.Color stoneColor, bool isTurnPlayer)
                {
                    Id = id;
                    Name = name;
                    StoneColor = stoneColor;
                    IsTurnPlayer = isTurnPlayer;
                }
            }
        }

        public readonly struct Stone
        {
            public Point Point { get; }

            public Color StoneColor { get; }

            public Stone(Point point, Color color = Color.None)
            {
                Point = point;
                StoneColor = color;
            }

            public enum Color
            {
                Light,
                Dark,
                None
            }
        }

        public readonly struct Point
        {
            public int X { get; }

            public int Y { get; }

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
    }
}
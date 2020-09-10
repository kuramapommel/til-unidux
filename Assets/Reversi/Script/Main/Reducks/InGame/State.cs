using System;
using System.Linq;
using Pommel.Reversi.Domain.InGame;
using Unidux;

namespace Pommel.Reversi.Reducks.InGame
{
    public interface IProps
    {
        string Id { get; }

        ValueObjects.Room Room { get; }

        Elements.Board.IProps Board { get; }

        ValueObjects.State State { get; }
    }

    [Serializable]
    public abstract class InGameState : StateElement
    {
        public abstract string Id { get; set; }

        public abstract ValueObjects.Room Room { get; set;  }

        public abstract Elements.Board.State Board { get; }

        public abstract ValueObjects.State State { get; set; }
    }

    public static class State
    {
        private static readonly Lazy<Impl> m_instance = new Lazy<Impl>(() => new Impl());

        public static InGameState Element => m_instance.Value;

        public static IProps Props => m_instance.Value;

        [Serializable]
        private sealed class Impl : InGameState, IProps
        {
            public override ValueObjects.Room Room { get; set; } = new ValueObjects.Room(
                string.Empty,
                new ValueObjects.Room.Player(
                    string.Empty,
                    string.Empty,
                    ValueObjects.Stone.Color.Dark,
                    true
                    ),
                new ValueObjects.Room.Player(
                    string.Empty,
                    string.Empty,
                    ValueObjects.Stone.Color.Light,
                    false
                    )
                );

            public override Elements.Board.State Board => Elements.Board.Element;

            public override string Id { get; set; } = string.Empty;

            public override ValueObjects.State State { get; set; } = ValueObjects.State.NotYet;

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
                ValueObjects.Stone[] Stones { get; }
            }

            public abstract class State : StateElement
            {
                public abstract ValueObjects.Stone[] Stones { get; set; }
            }

            private sealed class Impl : State, IProps
            {
                public override ValueObjects.Stone[] Stones { get; set; } = Enumerable.Range(0, 8)
                    .SelectMany(x => Enumerable.Range(0, 8)
                    .Select(y => new ValueObjects.Stone(new ValueObjects.Point(x, y))))
                    .ToArray();
            }
        }
    }
}
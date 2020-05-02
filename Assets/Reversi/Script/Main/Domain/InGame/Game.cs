using System.Collections.Generic;

namespace Pommel.Reversi.Domain.InGame
{
    // 試合集約
    public interface IGame
    {
        // todo 型作る
        string Id { get; }

        // todo 型作る
        string ResultId { get; }

        State State { get; }

        IEnumerable<Stone> Stones { get; }

        IGame PutStone(Point point);
    }

    public sealed class Game : IGame
    {
        public string Id { get; }

        public string ResultId { get; }

        public State State { get; }

        public IEnumerable<Stone> Stones { get; }
        public IGame PutStone(Point point) => throw new System.NotImplementedException();
    }

    public enum State
    {
        NotYet,
        Playing,
        GameSet
    }
}
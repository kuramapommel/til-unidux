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

        bool IsGameSet { get; }

        IEnumerable<Stone> Stones { get; }

        IGame PutStone(Point point);
    }

    public sealed class Game : IGame
    {
        public string Id { get; }

        public string ResultId { get; }

        public bool IsGameSet { get; }

        public IEnumerable<Stone> Stones { get; }
        public IGame PutStone(Point point) => throw new System.NotImplementedException();
    }
}
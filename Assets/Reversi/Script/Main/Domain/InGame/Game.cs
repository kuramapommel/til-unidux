using System.Collections.Generic;

namespace Pommel.Reversi.Domain.InGame
{
    // 試合集約
    public interface IGame
    {
        // todo 型作る
        string Id {get;}

        // todo 型作る
        string ResultId {get;}

        IDictionary<Point, Stone> Stones {get;}

        IGame PutStone(Point point);
    }

    public sealed class Game : IGame
    {
        public string Id { get; }

        public string ResultId { get; }

        public IDictionary<Point, Stone> Stones {get;}
        public IGame PutStone(Point point) => throw new System.NotImplementedException();
    }

}
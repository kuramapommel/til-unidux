using System.Collections.Generic;

namespace Pommel.Reversi.Domain.InGame
{
    // 試合集約
    public interface IGame
    {
        // todo 型作る
        string Id { get; }

        string NextTurnPlayerId { get; }

        IEnumerable<Piece> Pieces { get; }
    }

    public interface IGameFactory
    {
        IGame Create(string id, string nextTurnPlayerId, IEnumerable<Piece> pieces);
    }

    public sealed class Game : IGame
    {
        public string Id { get; }

        public string NextTurnPlayerId { get; }

        public IEnumerable<Piece> Pieces { get; }

        public Game(string id, string nextTurnPlayerId, IEnumerable<Piece> pieces)
        {
            Id = id;
            NextTurnPlayerId = nextTurnPlayerId;
            Pieces = pieces;
        }
    }
}
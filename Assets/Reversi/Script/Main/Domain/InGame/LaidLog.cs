using System.Collections.Generic;

namespace Pommel.Reversi.Domain.InGame
{
    public readonly struct LaidLog
    {
        public string LaidPlayerId { get; }

        public string NextPlayerId { get; }

        public Point Point { get; }

        public IEnumerable<Piece> Pieces { get; }

        public LaidLog(
            string laidPlayerId,
            string nextPlayerId,
            Point point,
            IEnumerable<Piece> pieces
            )
        {
            LaidPlayerId = laidPlayerId;
            NextPlayerId = nextPlayerId;
            Point = point;
            Pieces = pieces;
        }
    }
}
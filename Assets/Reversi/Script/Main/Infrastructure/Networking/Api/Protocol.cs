using MessagePack;

namespace Pommel.Api.Protocol
{
    namespace InGame
    {
        [MessagePackObject]
        public sealed class Game
        {
            [Key(0)]
            public string Id { get; set; }

            [Key(1)]
            public Piece[] Pieces { get; set; }
        }

        [MessagePackObject]
        public sealed class Piece
        {
            [Key(0)]
            public int X { get; set; }

            [Key(1)]
            public int Y { get; set; }

            [Key(2)]
            public int Color { get; set; }
        }
    }
}
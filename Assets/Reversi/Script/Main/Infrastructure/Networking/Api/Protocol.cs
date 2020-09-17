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

            [Key(2)]
            public Room Room { get; set; }

            [Key(3)]
            public int State { get; set; } 
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

        [MessagePackObject]
        public sealed class Room
        {
            [Key(0)]
            public Player FirstPlayer { get; set; }

            [Key(1)]
            public Player SecondPlayer { get; set; }
        }

        [MessagePackObject]
        public sealed class Player
        {
            [Key(0)]
            public string Id { get; set; }

            [Key(1)]
            public string Name { get; set; }

            [Key(2)]
            public bool IsTurnPlayer { get; set; }

            [Key(3)]
            public bool IsLight { get; set; }
        }
    }
}
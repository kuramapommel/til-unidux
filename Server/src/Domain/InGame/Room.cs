using System;
using System.Linq;

namespace Pommel.Server.Domain.InGame
{
    public sealed class Room
    {
        public IPlayer FirstPlayer { get; } = Player.None;

        public IPlayer SecondPlayer { get; } = Player.None;

        public Room()
        {
            FirstPlayer = Player.None;
            SecondPlayer = Player.None;
        }

        public Room Enter(IPlayer player) => FirstPlayer == Player.None
            ? new Room(player, SecondPlayer)
            : new Room(FirstPlayer, player);

        public Room Make()
        {
            // 先攻後攻をシャッフル
            var (first, second) = new[]
            {
                FirstPlayer,
                SecondPlayer
            }
            .OrderBy(_ => Guid.NewGuid())
            .Aggregate(
                (firstPlayer: Player.None, secondPlayer: Player.None),
                (aggregate, player) => aggregate.firstPlayer == Player.None
                    ? (player, aggregate.secondPlayer)
                    : (aggregate.firstPlayer, player)
            );

            return new Room(first, second);
        }

        private Room(IPlayer first, IPlayer second)
        {
            FirstPlayer = first;
            SecondPlayer = second;
        }
    }
}
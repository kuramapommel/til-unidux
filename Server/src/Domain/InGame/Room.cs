using System;
using System.Linq;

namespace Pommel.Server.Domain.InGame
{
    public interface IRoom
    {
        string Id { get; }

        IPlayer FirstPlayer { get; }

        IPlayer SecondPlayer { get; }

        IRoom Enter(IPlayer player);

        IRoom Make();
    }

    public interface IRoomFactory
    {
        IRoom Create(string id);
    }

    public sealed class Room : IRoom
    {
        public string Id { get; }

        public IPlayer FirstPlayer { get; }

        public IPlayer SecondPlayer { get; }

        public Room(string id)
        {
            Id = id;
            FirstPlayer = Player.None;
            SecondPlayer = Player.None;
        }

        public IRoom Enter(IPlayer player) => FirstPlayer == Player.None
            ? new Room(Id, player, SecondPlayer)
            : new Room(Id, FirstPlayer, player);

        public IRoom Make()
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
                (aggregate, player) =>
                {
                    if (aggregate.firstPlayer == Player.None) return (player, aggregate.secondPlayer);
                    return (aggregate.firstPlayer, player);
                }
            );

            return new Room(Id, first, second);
        }

        private Room(string id, IPlayer first, IPlayer second)
        {
            Id = id;
            FirstPlayer = first;
            SecondPlayer = second;
        }
    }
}
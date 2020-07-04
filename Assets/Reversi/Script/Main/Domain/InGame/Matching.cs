using System;
using System.Linq;

namespace Pommel.Reversi.Domain.InGame
{
    public interface IMatching
    {
        string Id { get; }

        IPlayer FirstPlayer { get; }

        IPlayer SecondPlayer { get; }

        IMatching Entry(IPlayer second);

        IMatching Make();
    }

    public interface IMatchingFactory
    {
        IMatching Create(string id, IPlayer first);
    }

    public sealed class Matching : IMatching
    {
        public string Id { get; }

        public IPlayer FirstPlayer { get; }

        public IPlayer SecondPlayer { get; }

        public Matching(string id, IPlayer first)
        {
            Id = id;
            FirstPlayer = first;
            SecondPlayer = Player.None;
        }

        public IMatching Create(IPlayer first) => new Matching(Id, first, SecondPlayer);

        public IMatching Entry(IPlayer second) => new Matching(Id, FirstPlayer, second);

        public IMatching Make()
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

            return new Matching(Id, first, second);
        }

        private Matching(string id, IPlayer first, IPlayer second)
        {
            Id = id;
            FirstPlayer = first;
            SecondPlayer = second;
        }
    }
}
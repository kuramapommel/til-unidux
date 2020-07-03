using System;
using System.Linq;

namespace Pommel.Reversi.Domain.InGame
{
    public interface IMatching
    {
        string Id { get; }

        IPlayer FirstPlayer { get; }

        IPlayer SecondPlayer { get; }

        IMatching Create(IPlayer first);

        IMatching Join(IPlayer second);

        IMatching Make();
    }

    public sealed class Matching : IMatching
    {
        public string Id { get; }

        public IPlayer FirstPlayer { get; }

        public IPlayer SecondPlayer { get; }

        public IMatching Create(IPlayer first) => new Matching(first, SecondPlayer);

        public IMatching Join(IPlayer second) => new Matching(FirstPlayer, second);

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

            return new Matching(first, second);
        }

        private Matching(IPlayer first, IPlayer second)
        {
            FirstPlayer = first;
            SecondPlayer = second;
        }
    }
}
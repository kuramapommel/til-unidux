using System.Collections.Generic;
using System.Linq;
using Pommel.Reversi.Domain.InGame;

namespace Pommel.Reversi.Presentation.Scene.InGame.State
{
    public interface IGameBoardState
    {
        IEnumerable<IStoneState> Stones { get; }

        void Refresh(IEnumerable<Stone> stones);
    }

    public sealed class GameBoard : IGameBoardState
    {
        public IEnumerable<IStoneState> Stones { get; }

        public GameBoard(IEnumerable<IStoneState> stones)
        {
            Stones = stones.ToList();
        }

        public void Refresh(IEnumerable<Stone> stones)
        {
            foreach (var (stone, state) in stones
                .Join(
                    Stones,
                    stone => stone.Point,
                    state => state.Point,
                    (stone, state) => (stone, state)
                ))
            {
                state.SetColor(stone.Color.Convert());
            }
        }
    }
}
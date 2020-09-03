using System;
using System.Linq;

namespace Pommel.Reversi.Reducks.InGame
{
    public static class Selectors
    {
        public static Func<IState, ValueObject.Point, ValueObject.Stone> GetStone = (state, point) => state.InGame.Board.Stones.FirstOrDefault(stone => stone.Point.Equals(point));

        public static Func<IState, bool, ValueObject.Room.Player> GetPlayer = (state, isFirst) => isFirst ? state.InGame.Room.FirstPlayer : state.InGame.Room.SecondPlayer;
    }
}
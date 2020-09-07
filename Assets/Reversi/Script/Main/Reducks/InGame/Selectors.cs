using System;
using System.Linq;
using Pommel.Reversi.Domain.InGame;

namespace Pommel.Reversi.Reducks.InGame
{
    public static class Selectors
    {
        public static Func<StateRoot, ValueObjects.Point, ValueObjects.Stone> GetStone = (state, point) => state.InGame.Board.Stones.FirstOrDefault(stone => stone.Point.Equals(point));

        public static Func<StateRoot, ValueObjects.Stone[]> GetStones = state => state.InGame.Board.Stones;

        public static Func<StateRoot, bool, ValueObjects.Room.Player> GetPlayer = (state, isFirst) => isFirst ? state.InGame.Room.FirstPlayer : state.InGame.Room.SecondPlayer;

        public static Func<StateRoot, ValueObjects.State> GetGameState = state => state.InGame.State;
    }
}
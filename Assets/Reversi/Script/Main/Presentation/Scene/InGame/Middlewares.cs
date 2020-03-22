using System;
using Unidux;
using _StoneAction = Pommel.Reversi.Presentation.Scene.InGame.StoneAction.Action;

namespace Pommel.Reversi.Presentation.Scene.InGame
{
    public static class Middlewares
    {
        public static Func<Func<object, object>, Func<object, object>> ChangeTurn(IStoreObject store)
        {
            return (Func<object, object> next) => (object action) =>
            {
                if (!(action is _StoneAction stoneAction)) return next(action);

                var state = Unidux.State;
                var isBlackTurn = state.Turn.IsBlackTurn;
                var canPut = state.Stones.CanPut(stoneAction.X, stoneAction.Y, isBlackTurn);

                var result = next(stoneAction);
                var isTurnChange = isBlackTurn
                    ? canPut && state.Stones.CanPutWhite()
                    : canPut && state.Stones.CanPutBalck();

                if (!isTurnChange) return result;

                state.Turn.IsBlackTurn = !isBlackTurn;
                return result;
            };
        }

        public static Func<Func<object, object>, Func<object, object>> Result(IStoreObject store)
        {
            return (Func<object, object> next) => (object action) =>
            {
                return next(action);
            };
        }
    }
}
using System;
using Unidux;

namespace Pommel.Reversi.Presentation.Scene.InGame
{
    public static class Middlewares
    {
        public static Func<Func<object, object>, Func<object, object>> ChangeTurn(IStoreObject store)
        {
            return (Func<object, object> next) => (object action) =>
            {
                var state = Unidux.State;
                var isBlackTurn = state.Turn.IsBlackTurn;
                switch (isBlackTurn)
                {
                    case true when state.Stones.CanPutWhite():
                        state.Turn.IsBlackTurn = !isBlackTurn;
                        return next(action);

                    case false when state.Stones.CanPutBalck():
                        state.Turn.IsBlackTurn = !isBlackTurn;
                        return next(action);
                }
                return next(action);
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
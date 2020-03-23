using System;
using System.Linq;
using Unidux;
using _Middleware = Unidux.Middleware;
using _StoneAction = Pommel.Reversi.Presentation.Scene.InGame.StoneAction.Action;

namespace Pommel.Reversi.Presentation.Scene.InGame
{
    public interface IMiddlewares
    {
        _Middleware[] Collection { get; }
    }

    public sealed class Middlewares : IMiddlewares
    {
        private readonly IState m_state;

        public Middlewares(IState state) => m_state = state;

        public _Middleware[] Collection =>
            new _Middleware[]
            {
                ChangeTurn,
                Result
            };

        private Func<Func<object, object>, Func<object, object>> ChangeTurn(IStoreObject store)
        {
            return (Func<object, object> next) => (object action) =>
            {
                if (!(action is _StoneAction stoneAction)) return next(action);

                var isBlackTurn = m_state.Turn.IsBlackTurn;
                var canPut = m_state.Stones.CanPut(stoneAction.X, stoneAction.Y, isBlackTurn);

                var result = next(stoneAction);
                var isTurnChange = isBlackTurn
                    ? canPut && m_state.Stones.CanPutWhite()
                    : canPut && m_state.Stones.CanPutBalck();

                if (!isTurnChange) return result;

                m_state.Turn.IsBlackTurn = !isBlackTurn;
                return result;
            };
        }

        private Func<Func<object, object>, Func<object, object>> Result(IStoreObject store)
        {
            return (Func<object, object> next) => (object action) =>
            {
                if (!(action is _StoneAction stoneAction)) return next(action);

                var result = next(action);
                if (m_state.Stones.CanPutWhite() || m_state.Stones.CanPutBalck()) return result;

                var (black, white) = m_state.Stones
                    .SelectMany(stones => stones)
                    .Aggregate(
                        (black: 0, white: 0),
                        (aggregate, element) =>
                        {
                            switch (element.Color)
                            {
                                case StoneStateElement.State.Black: return (aggregate.black + 1, aggregate.white);
                                case StoneStateElement.State.White: return (aggregate.black, aggregate.white + 1);
                            }

                            return aggregate;
                        });

                m_state.Result.Winner = (white > black)
                    ? WinnerStateElement.State.White
                    : (black > white)
                        ? WinnerStateElement.State.Black
                        : WinnerStateElement.State.Draw;

                return result;
            };
        }
    }
}
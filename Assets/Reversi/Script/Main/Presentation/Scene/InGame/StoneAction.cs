using System;
using Unidux;
using System.Linq;

namespace Pommel.Reversi.Presentation.Scene.InGame
{
    public static class StoneAction
    {
        public enum ActionType
        {
            Put
        }

        public sealed class Action
        {
            public ActionType Type { get; }

            public int X { get; }

            public int Y { get; }

            public Action(ActionType type, int x, int y) =>
                (Type, X, Y) =
                (type, x, y);
        }

        public static class ActionCreator
        {
            public static Action Put(int x, int y) => new Action(ActionType.Put, x, y);
        }

        public sealed class Reducer : ReducerBase<State, Action>
        {
            public override State Reduce(State state, Action action)
            {
                var stone = state.Stones[action.X][action.Y];
                switch (action.Type)
                {
                    case ActionType.Put when !state.Stones.CanPut(action.X, action.Y, state.Turn.IsBlackTurn): return state;

                    case ActionType.Put:
                        stone.Color = state.Turn.IsBlackTurn
                            ? StoneStateElement.State.Black
                            : StoneStateElement.State.White;
                        state.Stones[action.X][action.Y] = stone;
                        state.Stones.Flip(action.X, action.Y, state.Turn.IsBlackTurn);

                        if (state.Stones.CanPut(state.Turn.IsBlackTurn))
                        {
                            state.Turn.IsBlackTurn = !state.Turn.IsBlackTurn;
                            return state;
                        }

                        if (state.Stones.CanPut(!state.Turn.IsBlackTurn)) return state;

                        var (black, white) = state.Stones
                            .SelectMany(stones => stones)
                            .Aggregate(
                                (blackCount: 0, whiteCount: 0),
                                (aggregate, element) =>
                                {
                                    switch (element.Color)
                                    {
                                        case StoneStateElement.State.Black: return (aggregate.blackCount + 1, aggregate.whiteCount);
                                        case StoneStateElement.State.White: return (aggregate.blackCount, aggregate.whiteCount + 1);
                                    }

                                    return aggregate;
                                });

                        state.Result.Winner = (white > black)
                            ? WinnerStateElement.State.White
                            : (black > white)
                                ? WinnerStateElement.State.Black
                                : WinnerStateElement.State.Draw;
                        return state;

                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}

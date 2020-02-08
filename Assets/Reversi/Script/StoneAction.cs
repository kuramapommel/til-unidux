using System;
using Unidux;

namespace Pommel.Reversi
{
    public static class StoneAction
    {
        public enum ActionType
        {
            Put,
            TurnOver
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

            public static Action TurnOver(int x, int y) => new Action(ActionType.TurnOver, x, y);
        }

        public sealed class Reducer : ReducerBase<State, Action>
        {
            public override State Reduce(State state, Action action)
            {
                StoneStateElement changeStoneColor(StoneStateElement stone)
                {
                    switch (action.Type)
                    {
                        case ActionType.Put:
                            stone.Color = state.Turn.IsBlackTurn
                                ? StoneStateElement.State.Black
                                : StoneStateElement.State.White;
                            state.Turn.IsBlackTurn = !state.Turn.IsBlackTurn;
                            return stone;

                        case ActionType.TurnOver when stone.Color == StoneStateElement.State.Black:
                            stone.Color = StoneStateElement.State.White;
                            return stone;

                        case ActionType.TurnOver when stone.Color == StoneStateElement.State.White:
                            stone.Color = StoneStateElement.State.Black;
                            return stone;

                        default: throw new ArgumentOutOfRangeException();
                    }
                }

                state.Stones[action.X][action.Y] = changeStoneColor(state.Stones[action.X][action.Y]);
                return state;
            }
        }
    }
}

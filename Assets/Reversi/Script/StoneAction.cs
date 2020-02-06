using System;
using Unidux;
using System.Linq;

namespace Pommel.Reversi
{
    public static class StoneAction
    {
        public enum ActionType
        {
            PutBlack,
            PutWhite,
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
            public static Action PutBlack(int x, int y) => new Action(ActionType.PutBlack, x, y);

            public static Action PutWhite(int x, int y) => new Action(ActionType.PutWhite, x, y);

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
                        case ActionType.PutBlack:
                            stone.Color = StoneStateElement.State.Black;
                            return stone;

                        case ActionType.PutWhite:
                            stone.Color = StoneStateElement.State.White;
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
                foreach (var element in state.Stones.SelectMany(stones => stones)) element.IsBlackTurn = !element.IsBlackTurn;
                return state;
            }
        }
    }
}

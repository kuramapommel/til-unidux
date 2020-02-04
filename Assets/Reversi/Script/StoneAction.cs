using System;
using Unidux;

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
        }

        public sealed class Reducer : ReducerBase<State, Action>
        {
            public override State Reduce(State state, Action action)
            {
                var stone = state.Stones[action.X][action.Y];
                switch (action.Type)
                {
                    case ActionType forWhite when
                            ActionType.PutBlack == forWhite
                            || (ActionType.TurnOver == forWhite && stone.Color == StoneStateElement.State.Black):

                        stone.Color = StoneStateElement.State.White;
                        state.Stones[action.X][action.Y] = stone;
                        return state;

                    case ActionType forBlack when
                            ActionType.PutWhite == forBlack
                            || (ActionType.TurnOver == forBlack && stone.Color == StoneStateElement.State.White):

                        stone.Color = StoneStateElement.State.Black;
                        state.Stones[action.X][action.Y] = stone;
                        return state;
                }

                throw new ArgumentOutOfRangeException();
            }
        }
    }
}

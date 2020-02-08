﻿using System;
using Unidux;

namespace Pommel.Reversi
{
    public static class StoneAction
    {
        public enum ActionType
        {
            Put,
            Flip
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

            public static Action Flip(int x, int y) => new Action(ActionType.Flip, x, y);
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
                        state.Turn.IsBlackTurn = !state.Turn.IsBlackTurn;
                        state.Stones[action.X][action.Y] = stone;
                        return state;

                    case ActionType.Flip when stone.Color == StoneStateElement.State.Black:
                        stone.Color = StoneStateElement.State.White;
                        state.Stones[action.X][action.Y] = stone;
                        return state;

                    case ActionType.Flip when stone.Color == StoneStateElement.State.White:
                        stone.Color = StoneStateElement.State.Black;
                        state.Stones[action.X][action.Y] = stone;
                        return state;

                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}

using System;

namespace Pommel.Reversi.Reducks.Title
{
    public static class Actions
    {
        public enum ActionType
        {
            OpenGameStartModalAction,
        }

        public static Func<bool, IAction<ActionType>> OpenGameStartModalAction = payload => new ActionCommand<ActionType, bool>(ActionType.OpenGameStartModalAction, payload);
    }
}
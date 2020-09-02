using System;

namespace Pommel.Reversi.Scene.Title
{
    public static class Actions
    {
        public enum ActionType
        {
            OpenGameStartModalAction,
        }

        public static Func<int, IAction> OpenGameStartModalAction = (int payload) => new ActionCommand<ActionType, int>(ActionType.OpenGameStartModalAction, payload);
    }
}
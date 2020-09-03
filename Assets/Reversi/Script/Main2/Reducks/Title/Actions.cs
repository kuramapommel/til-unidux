using System;

namespace Pommel.Reversi.Reducks.Title
{
    public static class Actions
    {
        public enum ActionType
        {
            OpenGameStartModalAction,
            InputPlayerIdAction,
            InputPlayerNameAction,
            InputRoomIdAction,
        }

        public static Func<bool, IAction<ActionType>> OpenGameStartModalAction = (bool payload) => new ActionCommand<ActionType, bool>(ActionType.OpenGameStartModalAction, payload);

        public static Func<string, IAction<ActionType>> InputPlayerIdAction = (string payload) => new ActionCommand<ActionType, string>(ActionType.InputPlayerIdAction, payload);

        public static Func<string, IAction<ActionType>> InputPlayerNameAction = (string payload) => new ActionCommand<ActionType, string>(ActionType.InputPlayerNameAction, payload);

        public static Func<string, IAction<ActionType>> InputRoomIdAction = (string payload) => new ActionCommand<ActionType, string>(ActionType.InputRoomIdAction, payload);
    }
}
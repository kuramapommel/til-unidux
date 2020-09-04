using System;

namespace Pommel.Reversi.Reducks.InGame
{
    public static class Actions
    {
        public enum ActionType
        {
            CreateRoomAction,
            EntryGameAction,
            RefreshBoardAction,
            SwitchTurnAction
        }

        public static Func<ValueObject.Room, IAction<ActionType>> CreateRoomAction = payload => new ActionCommand<ActionType, ValueObject.Room>(ActionType.CreateRoomAction, payload);

        public static Func<ValueObject.Room.Player, IAction<ActionType>> EntryGameAction = payload => new ActionCommand<ActionType, ValueObject.Room.Player>(ActionType.EntryGameAction, payload);

        public static Func<ValueObject.Stone[], IAction<ActionType>> RefreshBoardAction = payload => new ActionCommand<ActionType, ValueObject.Stone[]>(ActionType.RefreshBoardAction, payload);

        public static Func<string, IAction<ActionType>> SwitchTurnAction = payload => new ActionCommand<ActionType, string>(ActionType.RefreshBoardAction, payload);
    }
}
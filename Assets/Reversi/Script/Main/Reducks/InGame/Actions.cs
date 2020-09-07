using System;
using Pommel.Reversi.Domain.InGame;

namespace Pommel.Reversi.Reducks.InGame
{
    public static class Actions
    {
        public enum ActionType
        {
            CreateRoomAction,
            EntryRoomAction,
            RefreshGameAction
        }

        public static Func<ValueObjects.Room, IAction<ActionType>> CreateRoomAction = payload => new ActionCommand<ActionType, ValueObjects.Room>(ActionType.CreateRoomAction, payload);

        public static Func<ValueObjects.Room.Player, IAction<ActionType>> EntryRoomAction = payload => new ActionCommand<ActionType, ValueObjects.Room.Player>(ActionType.EntryRoomAction, payload);

        public static Func<ValueObjects.Game, IAction<ActionType>> RefreshGameAction = payload => new ActionCommand<ActionType, ValueObjects.Game>(ActionType.RefreshGameAction, payload);
    }
}
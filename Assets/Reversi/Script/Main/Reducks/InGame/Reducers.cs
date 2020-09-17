using Pommel.Reversi.Domain.InGame;
using Unidux;

namespace Pommel.Reversi.Reducks.InGame
{
    using static Actions.ActionType;

    public sealed class Reducer : ReducerBase<StateRoot, IAction<Actions.ActionType>>
    {
        public override StateRoot Reduce(StateRoot state, IAction<Actions.ActionType> action)
        {
            switch (action.Type)
            {
                case CreateRoomAction when action is ActionCommand<Actions.ActionType, ValueObjects.Room> command:
                    {
                        state.InGame.Room = command.Payload;
                        return state;
                    }
                case EntryRoomAction when action is ActionCommand<Actions.ActionType, ValueObjects.Room.Player> command:
                    {
                        state.InGame.Room = new ValueObjects.Room(
                            state.InGame.Room.FirstPlayer,
                            command.Payload
                            );
                        return state;
                    }
                case RefreshGameAction when action is ActionCommand<Actions.ActionType, ValueObjects.Game> command:
                    {
                        state.InGame.Board.Stones = command.Payload.Stones;
                        state.InGame.Room = command.Payload.Room;
                        state.InGame.State = command.Payload.State;

                        return state;
                    }
            }
            return state;
        }
    }
}
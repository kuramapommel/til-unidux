using Unidux;
using StateRoot = Pommel.IState;

namespace Pommel.Reversi.Reducks.InGame
{
    using static Actions.ActionType;

    public sealed class Reducer : ReducerBase<StateRoot, IAction<Actions.ActionType>>
    {
        public override StateRoot Reduce(StateRoot state, IAction<Actions.ActionType> action)
        {
            switch (action.Type)
            {
                case CreateRoomAction when action is ActionCommand<Actions.ActionType, ValueObject.Room> command:
                    {
                        state.InGame.Room = command.Payload;
                        return state;
                    }
                case EntryGameAction when action is ActionCommand<Actions.ActionType, ValueObject.Room.Player> command:
                    {
                        state.InGame.Room = new ValueObject.Room(
                            state.InGame.Room.RoomId,
                            state.InGame.Room.FirstPlayer,
                            command.Payload
                            );
                        return state;
                    }
                case RefreshBoardAction when action is ActionCommand<Actions.ActionType, ValueObject.Stone[]> command:
                    {
                        state.InGame.Board.Stones = command.Payload;
                        return state;
                    }
            }
            return state;
        }
    }
}
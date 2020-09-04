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

                case SwitchTurnAction when action is ActionCommand<Actions.ActionType, string> command:
                    {
                        var (first, second) = command.Payload == state.InGame.Room.FirstPlayer.Id
                            ? (
                                first: new ValueObject.Room.Player(
                                    state.InGame.Room.FirstPlayer.Id,
                                    state.InGame.Room.FirstPlayer.Name,
                                    state.InGame.Room.FirstPlayer.StoneColor,
                                    true),
                                second: new ValueObject.Room.Player(
                                    state.InGame.Room.SecondPlayer.Id,
                                    state.InGame.Room.SecondPlayer.Name,
                                    state.InGame.Room.SecondPlayer.StoneColor,
                                    false)
                                )
                            : (
                                first: new ValueObject.Room.Player(
                                    state.InGame.Room.FirstPlayer.Id,
                                    state.InGame.Room.FirstPlayer.Name,
                                    state.InGame.Room.FirstPlayer.StoneColor,
                                    false
                                    ),
                                second: new ValueObject.Room.Player(
                                    state.InGame.Room.SecondPlayer.Id,
                                    state.InGame.Room.SecondPlayer.Name,
                                    state.InGame.Room.SecondPlayer.StoneColor,
                                    true)
                            );

                        state.InGame.Room = new ValueObject.Room(
                            state.InGame.Room.RoomId,
                            first,
                            second
                            );
                        return state;
                    }
            }
            return state;
        }
    }
}
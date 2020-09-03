using Unidux;
using StateRoot = Pommel.IState;

namespace Pommel.Reversi.Reducks.Title
{
    using static Actions.ActionType;

    public sealed class Reducer : ReducerBase<StateRoot, IAction<Actions.ActionType>>
    {
        public override StateRoot Reduce(StateRoot state, IAction<Actions.ActionType> action)
        {
            switch (action.Type)
            {
                case OpenGameStartModalAction when action is ActionCommand<Actions.ActionType, bool> command:
                    {
                        state.Title.IsDisplayGameStartModal = command.Payload;
                        return state;
                    }
                case InputPlayerIdAction when action is ActionCommand<Actions.ActionType, string> command:
                    {
                        var player = state.Title.Player;
                        state.Title.Player = new ValueObject.Player(command.Payload, player.Name);
                        return state;
                    }
                case InputPlayerNameAction when action is ActionCommand<Actions.ActionType, string> command:
                    {
                        var player = state.Title.Player;
                        state.Title.Player = new ValueObject.Player(player.Id, command.Payload);
                        return state;
                    }
                case InputRoomIdAction when action is ActionCommand<Actions.ActionType, string> command:
                    {
                        state.Title.RoomId = command.Payload;
                        return state;
                    }
            }
            return state;
        }
    }
}
using Unidux;

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
            }
            return state;
        }
    }
}
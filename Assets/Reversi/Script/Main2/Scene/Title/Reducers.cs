using Unidux;

namespace Pommel.Reversi.Scene.Title
{
    public sealed class Reducer : ReducerBase<State, IAction<Actions.ActionType>>
    {
        public override State Reduce(State state, IAction<Actions.ActionType> action)
        {
            switch (action.Type)
            {
                case Actions.ActionType.OpenGameStartModalAction when action is ActionCommand<Actions.ActionType, int> command:
                    state.Count = command.Payload;
                    return state;
            }
            return state;
        }
    }
}
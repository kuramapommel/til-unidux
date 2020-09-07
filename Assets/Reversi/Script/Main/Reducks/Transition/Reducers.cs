using Pommel.Reversi.Domain.Scene;
using Unidux.SceneTransition;

namespace Pommel.Reversi.Reducks.Scene
{
    public sealed class PageReducer : PageDuck<ValueObjects.Page, ValueObjects.Scene>.Reducer
    {
        public PageReducer() : base(new SceneConfig())
        {
        }
        
        public override object ReduceAny(object state, object action)
        {
            // It's required for detecting page scene state location
            var stateRoot = state as StateRoot;
            var pageAction = action as PageDuck<ValueObjects.Page, ValueObjects.Scene>.IPageAction;
            stateRoot.Page = Reduce(stateRoot.Page, stateRoot.Scene, pageAction);
            return state;
        }
    }

    public sealed class SceneReducer : SceneDuck<ValueObjects.Scene>.Reducer
    {
        public override object ReduceAny(object state, object action)
        {
            // It's required for detecting scene state location
            var stateRoot = state as StateRoot;
            var sceneAction = action as SceneDuck<ValueObjects.Scene>.Action;
            stateRoot.Scene = Reduce(stateRoot.Scene, sceneAction);
            return state;
        }
    }
}
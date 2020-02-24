using Unidux.SceneTransition;

namespace Pommel.Reversi.Presentation.Project.SceneChange
{
    public sealed class PageReducer : PageDuck<Page, Scene>.Reducer
    {
        public PageReducer() : base(new Config())
        {
        }

        public override object ReduceAny(object state, object action)
        {
            if (!(state is State changeSceneState) || !(action is PageDuck<Page, Scene>.IPageAction pageAction)) return state;

            changeSceneState.Page = Reduce(changeSceneState.Page, changeSceneState.Scene, pageAction);
            return changeSceneState;
        }
    }

    public sealed class SceneReducer : SceneDuck<Scene>.Reducer
    {
        public override object ReduceAny(object state, object action)
        {
            if (!(state is State changeSceneState) || !(action is SceneDuck<Scene>.Action pageAction)) return state;

            changeSceneState.Scene = Reduce(changeSceneState.Scene, pageAction);
            return changeSceneState;
        }
    }
}

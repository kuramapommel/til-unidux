using System.Collections.Generic;
using Unidux.SceneTransition;

namespace Pommel.Reversi.Presentation.Project.SceneChange
{
    public sealed class PageReducer : PageDuck<Page, Scene>.Reducer
    {
        public PageReducer() : base(new SceneConfig())
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

    public sealed class SceneConfig : ISceneConfig<Scene, Page>
    {
        private IDictionary<Scene, int> m_categoryMap;
        private IDictionary<Page, Scene[]> m_pageMap;

        public IDictionary<Scene, int> CategoryMap => m_categoryMap = m_categoryMap ?? new Dictionary<Scene, int>()
                {
                    { Scene.Base, SceneCategory.Permanent },
                    { Scene.Title, SceneCategory.Page },
                };

        public IDictionary<Page, Scene[]> PageMap => m_pageMap = m_pageMap ?? new Dictionary<Page, Scene[]>()
                {
                    { Page.TitlePage, new[] { Scene.Title } },
                    { Page.InGamePage, new[] { Scene.InGame } },
                };
    }
}

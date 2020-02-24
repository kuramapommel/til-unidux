using System.Collections.Generic;
using Unidux.SceneTransition;

namespace Pommel.Reversi.Presentation.Project.SceneChange
{
    public sealed class SceneConfig : ISceneConfig<Scene, Page>
    {
        private IDictionary<Scene, int> m_categoryMap;
        private IDictionary<Page, Scene[]> m_pageMap;

        public IDictionary<Scene, int> CategoryMap => m_categoryMap = m_categoryMap ?? new Dictionary<Scene, int>()
                {
                    { Scene.Base, SceneCategory.Permanent },
                    { Scene.Title, SceneCategory.Page },
                    { Scene.InGame, SceneCategory.Page },
                };

        public IDictionary<Page, Scene[]> PageMap => m_pageMap = m_pageMap ?? new Dictionary<Page, Scene[]>()
                {
                    { Page.TitlePage, new[] { Scene.Title } },
                    { Page.InGamePage, new[] { Scene.InGame } },
                };
    }

    public enum Page
    {
        TitlePage,
        InGamePage,
    }

    public enum Scene
    {
        Base,
        Title,
        InGame,
    }
}

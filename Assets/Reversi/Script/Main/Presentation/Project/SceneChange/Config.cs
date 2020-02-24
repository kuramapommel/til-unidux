using System;
using System.Collections.Generic;
using Unidux.SceneTransition;

namespace Pommel.Reversi.Presentation.Project.SceneChange
{
    public sealed class Config : ISceneConfig<Scene, Page>
    {
        private static readonly Lazy<IDictionary<Scene, int>> m_categoryMap = new Lazy<IDictionary<Scene, int>>(() => new Dictionary<Scene, int>()
                {
                    { Scene.Base, SceneCategory.Permanent },
                    { Scene.Title, SceneCategory.Page },
                    { Scene.InGame, SceneCategory.Page },
                });

        private static readonly Lazy<IDictionary<Page, Scene[]>> m_pageMap = new Lazy<IDictionary<Page, Scene[]>>(() => new Dictionary<Page, Scene[]>()
                {
                    { Page.TitlePage, new[] { Scene.Title } },
                    { Page.InGamePage, new[] { Scene.InGame } },
                });

        public IDictionary<Scene, int> CategoryMap => m_categoryMap.Value;

        public IDictionary<Page, Scene[]> PageMap => m_pageMap.Value;
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

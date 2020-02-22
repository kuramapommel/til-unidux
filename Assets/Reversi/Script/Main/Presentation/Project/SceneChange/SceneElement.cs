using System;
using Unidux;
using Unidux.SceneTransition;

namespace Pommel.Reversi.Presentation.Project.SceneChange
{
    [Serializable]
    public sealed class SceneStateElement : StateElement
    {
        public PageState<Page> Page { get; set; } = new PageState<Page>();
        public SceneState<Scene> Scene { get; set; } = new SceneState<Scene>();
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

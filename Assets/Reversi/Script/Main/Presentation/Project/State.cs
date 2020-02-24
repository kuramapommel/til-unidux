using System;
using Unidux;
using Unidux.SceneTransition;

namespace Pommel.Reversi.Presentation.Project
{
    [Serializable]
    public sealed class State : StateBase
    {
        public PageState<SceneChange.Page> Page = new PageState<SceneChange.Page>();
        public SceneState<SceneChange.Scene> Scene = new SceneState<SceneChange.Scene>();
    }
}

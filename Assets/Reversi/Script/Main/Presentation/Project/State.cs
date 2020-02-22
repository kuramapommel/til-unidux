using System;
using Unidux;
using Unidux.SceneTransition;
using SceneStateElement = Pommel.Reversi.Presentation.Project.SceneChange.SceneStateElement;

namespace Pommel.Reversi.Presentation.Project
{
    [Serializable]
    public sealed class State : StateBase
    {
        private static readonly Lazy<SceneStateElement> m_stateElement = new Lazy<SceneStateElement>(() => new SceneStateElement());

        public PageState<SceneChange.Page> Page
        {
            get => m_stateElement.Value.Page;
            set => m_stateElement.Value.Page = value;
        }

        public SceneState<SceneChange.Scene> Scene
        {
            get => m_stateElement.Value.Scene;
            set => m_stateElement.Value.Scene = value;
        }
    }
}

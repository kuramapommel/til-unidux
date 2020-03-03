using Pommel.Reversi.Presentation.Project.SceneChange;
using UniRx.Async;
using UnityEngine;

namespace Pommel.Reversi.Presentation.Scene.InGame
{
    public sealed class SceneRoot : SceneBase
    {
        protected override Page Page => Page.InGamePage;

        [SerializeField]
        private ResultRenderer m_resultRenderer;

        [SerializeField]
        private ResultDispatcher m_resultDispatcher;

        [SerializeField]
        private BoardRenderer m_boardRenderer;

        protected override async UniTask Initialize()
        {
            m_boardRenderer.Constructor();
        }

        protected override async UniTask OnOpen()
        {
            m_resultRenderer.Initialize();
            m_resultDispatcher.Initialize();
            m_boardRenderer.Initialize();
        }

        protected override async UniTask Dispose()
        {
        }
    }
}
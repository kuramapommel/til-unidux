using Pommel.Reversi.Presentation.Project.SceneChange;
using UniRx.Async;
using UnityEngine;

namespace Pommel.Reversi.Presentation.Scene.Title
{
    public sealed class SceneRoot : SceneBase
    {
        protected override Page Page => Page.TitlePage;

        [SerializeField]
        private StartDispatcher m_startDispatcher;

        protected override async UniTask Initialize()
        {
        }

        protected override async UniTask OnOpen()
        {
            m_startDispatcher.Initialize();
        }

        protected override async UniTask Dispose()
        {
        }
    }
}
using Pommel.Reversi.Presentation.Project.SceneChange;
using UniRx.Async;
using UnityEngine;
using Pommel.Reversi.Presentation.Scene.Title.UI;

namespace Pommel.Reversi.Presentation.Scene.Title
{
    public sealed class SceneRoot : SceneBase
    {
        protected override Page Page => Page.TitlePage;

        [SerializeField]
        private StartButton startButton;

        protected override async UniTask OnLoad()
        {
        }

        protected override async UniTask OnOpen()
        {
            startButton.Initialize();
        }

        protected override async UniTask OnDispose()
        {
            startButton.Dispose();
        }
    }
}
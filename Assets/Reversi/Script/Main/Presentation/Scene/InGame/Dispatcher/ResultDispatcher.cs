using System;
using Pommel.Reversi.Presentation.Project;
using Pommel.Reversi.Presentation.Project.SceneChange;
using Pommel.Reversi.Presentation.Scene.InGame.UI;
using UniRx;

namespace Pommel.Reversi.Presentation.Scene.InGame.Dispatcher
{
    public static class ResultDispatcher
    {
        public static IDisposable ApplyChangeSceneEvent(this ResultMessage message)
        {
            var disposable = message.Button.OnClickAsObservable()
                .Subscribe(action => GameCore.ChangeScene(Page.TitlePage))
                .AddTo(message);

            return disposable;
        }
    }
}
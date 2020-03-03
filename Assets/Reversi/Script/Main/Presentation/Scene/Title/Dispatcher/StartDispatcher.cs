using System;
using Pommel.Reversi.Presentation.Project;
using Pommel.Reversi.Presentation.Project.SceneChange;
using Pommel.Reversi.Presentation.Scene.Title.UI;
using UniRx;

namespace Pommel.Reversi.Presentation.Scene.Title.Dispatcher
{
    public static class StartDispatcher
    {
        public static IDisposable ApplyChangeSceneEvent(this StartButton button)
        {
            var disposable = button.Button.OnClickAsObservable()
                .Subscribe(action => GameCore.ChangeScene(Page.InGamePage))
                .AddTo(button);

            return disposable;
        }
    }
}

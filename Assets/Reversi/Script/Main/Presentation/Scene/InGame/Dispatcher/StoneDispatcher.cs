using System;
using Pommel.Reversi.Presentation.Scene.InGame.UI;
using UniRx;

namespace Pommel.Reversi.Presentation.Scene.InGame.Dispatcher
{
    public static class StoneDispatcher
    {
        public static IDisposable ApplyPutEvent(this Stone stone, int x, int y)
        {
            var disposable = stone.Button.OnClickAsObservable()
                .TakeUntilDisable(stone)
                .Where(_ => !stone.IsWhite && !stone.IsBlack)
                .Subscribe(_ => Unidux.Dispatch(StoneAction.ActionCreator.Put(x, y)))
                .AddTo(stone);

            return disposable;
        }
    }
}
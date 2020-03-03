using System;
using System.Linq;
using System.Threading;
using Pommel.Reversi.Presentation.Scene.InGame.UI;
using UniRx;

namespace Pommel.Reversi.Presentation.Scene.InGame.Renderer
{
    public static class ResultRenderer
    {
        public static IDisposable ApplyOpenResultMessageAnimation(this ResultMessage message)
        {
            var disposable = Unidux
                .Subject
                .TakeUntilDestroy(message)
                .StartWith(Unidux.State)
                .Where(state => state.Result.Winner != WinnerStateElement.State.Undecide)
                .Subscribe(async state =>
                {
                    string getResultMessage()
                    {
                        switch (state.Result.Winner)
                        {
                            case WinnerStateElement.State.Black: return "Black Win.";
                            case WinnerStateElement.State.White: return "White Win.";
                            case WinnerStateElement.State.Draw: return "Draw Game.";
                        }

                        throw new ArgumentOutOfRangeException();
                    }

                    message.SetMessageText(getResultMessage());

                    await message.Open(new CancellationTokenSource());
                })
                .AddTo(message);

            return disposable;
        }
    }
}
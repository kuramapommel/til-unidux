using System;
using UniRx;

namespace Pommel.Reversi.Presentation.ViewModel.Title
{
    public interface ITitleViewModel : IDisposable
    {
        IObservable<Unit> OnTapTitleAsObservable();

        void TapTitle();
    }

    public sealed class TitleViewModel : ITitleViewModel
    {
        private readonly ISubject<Unit> m_onTapTitleAsObservable = new Subject<Unit>();

        public IObservable<Unit> OnTapTitleAsObservable() => m_onTapTitleAsObservable;

        public void TapTitle() => m_onTapTitleAsObservable.OnNext(Unit.Default);

        void IDisposable.Dispose()
        {
            m_onTapTitleAsObservable.OnCompleted();
        }
    }
}

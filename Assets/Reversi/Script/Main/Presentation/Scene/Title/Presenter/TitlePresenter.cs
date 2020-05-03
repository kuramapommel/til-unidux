using Pommel.Reversi.Presentation.Project; // todo static に依存するのをやめる
using Pommel.Reversi.Presentation.Project.SceneChange;
using Pommel.Reversi.Presentation.Scene.Title.View;
using Pommel.Reversi.UseCase.InGame;
using UniRx;
using Zenject;

namespace Pommel.Reversi.Presentation.Scene.Title.Presenter
{
    public interface ITitlePresenter
    {
    }
    
    public sealed class TitlePresenter : ITitlePresenter, IInitializable
    {
        private readonly ITitleTapArea m_title;

        private readonly ICreateGameUseCase m_createGameUseCase;

        public TitlePresenter(
            ITitleTapArea titleTapArea,
            ICreateGameUseCase createGameUseCase
            )
        {
            m_title= titleTapArea;
            m_createGameUseCase = createGameUseCase;
        }

        public void Initialize()
        {
            m_title.OnTapAsObservable()
                .SelectMany(_ => m_createGameUseCase.Execute().ToObservable())
                .Subscribe(_ => GameCore.ChangeScene(Page.InGamePage));
        }
    }
}

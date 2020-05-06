using Pommel.Reversi.Presentation.Model.InGame;
using Zenject;

namespace Pommel.Reversi.Presentation.Presenter.InGame
{
    public interface IGamePresenter
    {
    }

    public sealed class GamePresenter : IGamePresenter, IInitializable
    {
        private readonly IGameModel m_gameModel;

        public GamePresenter(
            IGameModel gameModel
            )
        {
            m_gameModel = gameModel;
        }

        public void Initialize()
        {
            // todo 画面遷移後の初期化処理として呼べるようにできればベスト
            _ = m_gameModel.Start();
        }
    }

}
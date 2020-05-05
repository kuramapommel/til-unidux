using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Presentation.Model.InGame;
using Pommel.Reversi.Presentation.View.InGame;
using Pommel.Reversi.UseCase.InGame;
using Pommel.Reversi.UseCase.Shared;
using UniRx;
using UniRx.Async;
using Zenject;

namespace Pommel.Reversi.Presentation.Presenter.InGame
{
    public interface IPiecePresenter
    {
    }

    public sealed class PiecePresenter : IPiecePresenter, IInitializable
    {
        private readonly IFactory<IGameRepository, IEventPublisher, string, ILayPieceUseCase> m_layPieceUsecaseFactory;

        private readonly IGameRepository m_gameRepository;

        private readonly IGameService m_gameService;

        private readonly IEventPublisher m_eventPublisher;

        private readonly IGameModel m_gameModel;

        private readonly IGameBoard m_gameBoard;

        public PiecePresenter(
            IFactory<IGameRepository, IEventPublisher, string, ILayPieceUseCase> laypieceUsecaseFactory,
            IGameRepository gameRepository,
            IGameService gameService,
            IEventPublisher eventPublisher,
            IGameModel gameModel,
            IGameBoard gameBoard
            )
        {
            m_layPieceUsecaseFactory = laypieceUsecaseFactory;
            m_gameRepository = gameRepository;
            m_gameService = gameService;
            m_eventPublisher = eventPublisher;
            m_gameModel = gameModel;
            m_gameBoard = gameBoard;
        }

        public void Initialize()
        {
            m_gameModel.OnStart
                .ContinueWith(_ => m_gameService.FetchPlaying().ToObservable())
                .Subscribe(game =>
                {
                    var laypieceUsecase = m_layPieceUsecaseFactory.Create(
                                m_gameRepository,
                                m_eventPublisher,
                                game.Id
                            );

                    m_gameBoard.InstantiatePieces(
                        m_gameModel,
                        laypieceUsecase.Execute);
                },
                UnityEngine.Debug.Log);
        }
    }
}
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Presentation.Scene.InGame.State;
using Pommel.Reversi.Presentation.Scene.InGame.View;
using Pommel.Reversi.UseCase.InGame;
using Pommel.Reversi.UseCase.Shared;
using UniRx;
using UniRx.Async;
using Zenject;

namespace Pommel.Reversi.Presentation.Scene.InGame.Presenter
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

        private readonly IGameState m_gameState;

        private readonly IGameBoard m_gameBoard;

        public PiecePresenter(
            IFactory<IGameRepository, IEventPublisher, string, ILayPieceUseCase> laypieceUsecaseFactory,
            IGameRepository gameRepository,
            IGameService gameService,
            IEventPublisher eventPublisher,
            IGameState gameState,
            IGameBoard gameBoard
            )
        {
            m_layPieceUsecaseFactory = laypieceUsecaseFactory;
            m_gameRepository = gameRepository;
            m_gameService = gameService;
            m_eventPublisher = eventPublisher;
            m_gameState = gameState;
            m_gameBoard = gameBoard;
        }

        public void Initialize()
        {
            m_gameState.OnStart
                .ContinueWith(_ => m_gameService.FetchPlaying().ToObservable())
                .Subscribe(game =>
                {
                    var laypieceUsecase = m_layPieceUsecaseFactory.Create(
                                m_gameRepository,
                                m_eventPublisher,
                                game.Id
                            );

                    m_gameBoard.InstantiatePieces(
                        m_gameState,
                        laypieceUsecase.Execute);
                },
                UnityEngine.Debug.Log);
        }
    }
}
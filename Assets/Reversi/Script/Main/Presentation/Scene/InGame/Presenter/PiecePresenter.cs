using System;
using System.Collections.Generic;
using System.Linq;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Presentation.Scene.InGame.State;
using Pommel.Reversi.Presentation.Scene.InGame.View;
using Pommel.Reversi.UseCase.InGame;
using Pommel.Reversi.UseCase.InGame.Dto;
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
        private readonly IFactory<IEnumerable<IPieceState>, IGameBoardState> m_gameboardFactory;

        private readonly IFactory<Point, Color, IPieceState> m_pieceStateFactory;

        private readonly IFactory<IGameRepository, IEventPublisher, string, ILayPieceUseCase> m_layPieceUsecaseFactory;

        private readonly IFactory<IEventBroker, IEventPublisher> m_eventPublisherFactory;

        private readonly IFactory<Func<ResultDto, UniTask>, Func<LaidDto, UniTask>, IGameResultService, IEventSubscriber> m_eventSubscriberFactory;

        private readonly IGameRepository m_gameRepository;

        private readonly IGameService m_gameService;

        private readonly IGameResultService m_gameResultService;

        private readonly IEventBroker m_eventBroker;

        private readonly IGameBoard m_gameBoard;

        public PiecePresenter(
            IFactory<IEnumerable<IPieceState>, IGameBoardState> gameboardFactory,
            IFactory<Point, Color, IPieceState> pieceStateFactory,
            IFactory<IGameRepository, IEventPublisher, string, ILayPieceUseCase> laypieceUsecaseFactory,
            IFactory<IEventBroker, IEventPublisher> eventPublisherFactory,
            IFactory<Func<ResultDto, UniTask>, Func<LaidDto, UniTask>, IGameResultService, IEventSubscriber> eventSubscriberFactory,
            IGameRepository gameRepository,
            IGameService gameService,
            IGameResultService gameResultService,
            IEventBroker eventBroker,
            IGameBoard gameBoard
            )
        {
            m_gameboardFactory = gameboardFactory;
            m_pieceStateFactory = pieceStateFactory;
            m_layPieceUsecaseFactory = laypieceUsecaseFactory;
            m_eventPublisherFactory = eventPublisherFactory;
            m_eventSubscriberFactory = eventSubscriberFactory;
            m_gameRepository = gameRepository;
            m_gameService = gameService;
            m_gameResultService = gameResultService;
            m_eventBroker = eventBroker;
            m_gameBoard = gameBoard;
        }

        public void Initialize()
        {
            m_gameService.FetchPlaying()
                .ToObservable()
                .Subscribe(game =>
                {
                    // todo 駒の関心事から大きく溢れているので、もうちょっと共通なところでやる
                    var gameBoardState = m_gameboardFactory.Create(game.Pieces.Select(piece => m_pieceStateFactory.Create(piece.Point, piece.Color)));
                    m_eventBroker.RegisterSubscriber<ILaidPieceEvent>(
                        m_eventSubscriberFactory.Create(
                            async result => { },
                            async putted => gameBoardState.Refresh(putted.Pieces),
                            m_gameResultService));

                    var laypieceUsecase = m_layPieceUsecaseFactory.Create(
                                m_gameRepository,
                                m_eventPublisherFactory.Create(m_eventBroker),
                                game.Id
                            );
                    m_gameBoard.InstantiatePieces(
                        gameBoardState,
                        async point =>
                        {
                            var putted = await laypieceUsecase.Execute(point.X, point.Y);
                            return putted.Pieces;
                        });
                });
        }
    }
}
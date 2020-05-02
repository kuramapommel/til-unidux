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

namespace Pommel.Reversi.Presentation.Scene.InGame.Dispatcher
{
    public interface IStoneDispatcher
    {
    }

    public sealed class StoneDispatcher : IStoneDispatcher, IInitializable
    {
        private readonly IFactory<IEnumerable<IStoneState>, IGameBoardState> m_gameboardFactory;

        private readonly IFactory<Point, Color, IStoneState> m_stoneStateFactory;

        private readonly IFactory<IGameRepository, IEventPublisher, string, IPutStoneUseCase> m_putstoneUsecaseFactory;

        private readonly IFactory<IEventBroker, IEventPublisher> m_eventPublisherFactory;

        private readonly IFactory<Func<ResultDto, UniTask>, Func<PuttedDto, UniTask>, IGameResultService, IEventSubscriber> m_eventSubscriberFactory;

        private readonly IGameRepository m_gameRepository;

        private readonly IGameService m_gameService;

        private readonly IGameResultService m_gameResultService;

        private readonly IEventBroker m_eventBroker;

        private readonly IGameBoard m_gameBoard;

        public StoneDispatcher(
            IFactory<IEnumerable<IStoneState>, IGameBoardState> gameboardFactory,
            IFactory<Point, Color, IStoneState> stoneStateFactory,
            IFactory<IGameRepository, IEventPublisher, string, IPutStoneUseCase> putstoneUsecaseFactory,
            IFactory<IEventBroker, IEventPublisher> eventPublisherFactory,
            IFactory<Func<ResultDto, UniTask>, Func<PuttedDto, UniTask>, IGameResultService, IEventSubscriber> eventSubscriberFactory,
            IGameRepository gameRepository,
            IGameService gameService,
            IGameResultService gameResultService,
            IEventBroker eventBroker,
            IGameBoard gameBoard
            )
        {
            m_gameboardFactory = gameboardFactory;
            m_stoneStateFactory = stoneStateFactory;
            m_putstoneUsecaseFactory = putstoneUsecaseFactory;
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
                    var gameBoardState = m_gameboardFactory.Create(game.Stones.Select(stone => m_stoneStateFactory.Create(stone.Point, stone.Color)));
                    m_eventBroker.RegisterSubscriber<IPuttedStoneEvent>(
                        m_eventSubscriberFactory.Create(
                            async result => { },
                            async putted => gameBoardState.Refresh(putted.Stones),
                            m_gameResultService));

                    var putstoneUseCase = m_putstoneUsecaseFactory.Create(
                                m_gameRepository,
                                m_eventPublisherFactory.Create(m_eventBroker),
                                game.Id
                            );
                    m_gameBoard.InstantiateStones(
                        gameBoardState,
                        async point =>
                        {
                            var putted = await putstoneUseCase.Execute(point.X, point.Y);
                            return putted.Stones;
                        });
                });
        }
    }
}
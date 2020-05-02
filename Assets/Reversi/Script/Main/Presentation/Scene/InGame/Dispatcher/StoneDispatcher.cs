using System;
using System.Collections.Generic;
using System.Linq;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Presentation.Scene.InGame.State;
using Pommel.Reversi.Presentation.Scene.InGame.View;
using Pommel.Reversi.UseCase.InGame;
using Pommel.Reversi.UseCase.Shared;
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

        private readonly IFactory<IPuttedAdapter, IGameResultService, IEventSubscriber> m_eventSubscriberFactory;

        private readonly IFactory<Action<ResultDto>, Action<PuttedDto>, IPuttedAdapter> m_putAdapterFactory;

        private readonly IGameRepository m_gameRepository;

        private readonly IGameResultService m_gameResultService;

        private readonly IEventBroker m_eventBroker;

        private readonly IGameBoard m_gameBoard;

        public StoneDispatcher(
            IFactory<IEnumerable<IStoneState>, IGameBoardState> gameboardFactory,
            IFactory<Point, Color, IStoneState> stoneStateFactory,
            IFactory<IGameRepository, IEventPublisher, string, IPutStoneUseCase> putstoneUsecaseFactory,
            IGameRepository gameRepository,
            IGameBoard gameBoard
            )
        {
            // factories
            m_gameboardFactory = gameboardFactory;
            m_stoneStateFactory = stoneStateFactory;
            m_putstoneUsecaseFactory = putstoneUsecaseFactory;

            // repositories
            m_gameRepository = gameRepository;

            // views
            m_gameBoard = gameBoard;
        }

        public void Initialize()
        {
            // todo create game 的な usecase を作成して IGame を取得する
            IGame game = default;

            var gameBoardState = m_gameboardFactory.Create(game.Stones.Select(stone => m_stoneStateFactory.Create(stone.Point, stone.Color)));

            // todo 依存解決する
            m_eventBroker.RegisterSubscriber<IPuttedStoneEvent>(
                m_eventSubscriberFactory.Create(
                    m_putAdapterFactory.Create(
                        result => { },
                        putted => gameBoardState.Refresh(putted.Stones)
                        ),
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
        }
    }
}
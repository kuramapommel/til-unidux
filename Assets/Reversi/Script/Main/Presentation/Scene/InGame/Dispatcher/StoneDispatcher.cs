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

        private readonly IGameRepository m_gameRepository;

        private readonly IEventPublisher m_eventPublisher;

        private readonly IGameBoard m_gameBoard;

        public StoneDispatcher(
            IFactory<IEnumerable<IStoneState>, IGameBoardState> gameboardFactory,
            IFactory<Point, Color, IStoneState> stoneStateFactory,
            IFactory<IGameRepository, IEventPublisher, string, IPutStoneUseCase> putstoneUsecaseFactory,
            IGameRepository gameRepository,
            IEventPublisher eventPublisher,
            IGameBoard gameBoard
            )
        {
            // factories
            m_gameboardFactory = gameboardFactory;
            m_stoneStateFactory = stoneStateFactory;
            m_putstoneUsecaseFactory = putstoneUsecaseFactory;

            // repositories
            m_gameRepository = gameRepository;

            // utilities
            m_eventPublisher = eventPublisher;

            // views
            m_gameBoard = gameBoard;
        }

        public void Initialize()
        {
            // todo create game 的な usecase を作成して IGame を取得する
            IGame game = default;

            var putstoneUsecase = m_putstoneUsecaseFactory.Create(
                m_gameRepository,
                m_eventPublisher,
                game.Id
                );

            m_gameBoard.InstantiateStones(
                m_gameboardFactory.Create(game.Stones.Select(stone => m_stoneStateFactory.Create(stone.Point, stone.Color))),
                async point =>
                {
                    var putted = await putstoneUsecase.Execute(point.X, point.Y);
                    return putted.Stones;
                });
        }
    }
}
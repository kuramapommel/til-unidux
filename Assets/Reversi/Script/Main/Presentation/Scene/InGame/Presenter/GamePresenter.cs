using System;
using Pommel.Reversi.Presentation.Scene.InGame.State;
using Pommel.Reversi.UseCase.InGame;
using Pommel.Reversi.UseCase.InGame.Dto;
using Pommel.Reversi.UseCase.Shared;
using UniRx;
using UniRx.Async;
using Zenject;

namespace Pommel.Reversi.Presentation.Scene.InGame.Presenter
{
    public interface IGamePresenter
    {
    }

    public sealed class GamePresenter : IGamePresenter, IInitializable
    {
        private readonly IFactory<Func<ResultDto, UniTask>, Func<LaidDto, UniTask>, IGameResultService, IEventSubscriber> m_eventSubscriberFactory;

        private readonly IGameResultService m_gameResultService;

        private readonly IEventBroker m_eventBroker;

        private readonly IGameState m_gameState;

        private readonly IStartGameUseCase m_startGameUseCase;

        public GamePresenter(
            IFactory<Func<ResultDto, UniTask>, Func<LaidDto, UniTask>, IGameResultService, IEventSubscriber> eventSubscriberFactory,
            IGameResultService gameResultService,
            IEventBroker eventBroker,
            IGameState gameState,
            IStartGameUseCase startGameUseCase
            )
        {
            m_eventSubscriberFactory = eventSubscriberFactory;
            m_gameResultService = gameResultService;
            m_eventBroker = eventBroker;
            m_gameState = gameState;
            m_startGameUseCase = startGameUseCase;
        }

        public void Initialize()
        {
            m_eventBroker.RegisterSubscriber<ILaidPieceEvent>(
                m_eventSubscriberFactory.Create(
                    async result => { }, // todo result 処理の実装
                    async game => m_gameState.Refresh(game.Pieces),
                    m_gameResultService));

            m_startGameUseCase.Execute()
                .ToObservable()
                .Subscribe(
                    game => m_gameState.Start(game.Pieces),
                    UnityEngine.Debug.Log
                    );
        }
    }

}
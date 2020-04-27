using System;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.UseCase.InGame;
using Pommel.Reversi.UseCase.Shared;
using UniRx.Async;

namespace Pommel.Reversi.Api.InGame
{
    public interface IInGameApi
    {
        UniTask<Response> PutStone(Request request);
    }

    public sealed class InGameApi : IInGameApi
    {
        private readonly IGameRepository m_gameRepository;
        private readonly IEventPublisher m_eventPublisher;
        private readonly IEventSubscriber m_eventSubscriber;

        public InGameApi(
            IGameRepository gameRepository,
            IEventPublisher publisher
            )
        {
            m_gameRepository = gameRepository;
            m_eventPublisher = publisher;
        }

        public async UniTask<Response> PutStone(Request request)
        {
            // zenject の factory を使う
            var usecase = new PutStoneUseCase(m_gameRepository, m_eventPublisher, request.gameId, request.x, request.y);
            var gameTask = await usecase.Execute();
            return default;
        }
    }

    public readonly struct Request
    {
        public readonly string gameId;
        public readonly int x;
        public readonly int y;
    }

    public sealed class Response
    {
        public readonly IObservable<int> observable;
    }
}   
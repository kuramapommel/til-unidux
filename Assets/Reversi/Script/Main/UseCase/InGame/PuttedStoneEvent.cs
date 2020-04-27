using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.UseCase.Shared;
using UniRx.Async;

namespace Pommel.Reversi.UseCase.InGame
{
    public interface IPuttedStoneEvent : IEventMessage
    {
        IGame Game { get; }
    }

    public sealed class PuttedStoneEvent : IPuttedStoneEvent
    {
        public IGame Game { get; }

        public PuttedStoneEvent(IGame game) => Game = game;
    }

    public sealed class PuttedStoneEventSubscriber : IEventSubscriber
    {
        private readonly IPuttedAdapter m_puttedAdapter;

        private readonly IGameResultService m_gameResultService;

        public PuttedStoneEventSubscriber(IPuttedAdapter puttedAdapter, IGameResultService gameResultService)
        {
            m_puttedAdapter = puttedAdapter;
            m_gameResultService = gameResultService;
        }

        public async UniTask ReceivedMessage<EventMessage>(EventMessage message) where EventMessage : IEventMessage
        {
            if (!(message is PuttedStoneEvent puttedStoneEvent)) throw new System.AggregateException();

            var game = puttedStoneEvent.Game;
            if (game.IsGameSet)
            {
                var result = await m_gameResultService.FindById(game.ResultId);
                _ = UniTask.WhenAll(
                    m_puttedAdapter.OnResult(result),
                    m_puttedAdapter.OnPut(new PuttedDto(game.Stones))
                    );
                return;
            }

            _ = m_puttedAdapter.OnPut(new PuttedDto(game.Stones));
        }
    }
}
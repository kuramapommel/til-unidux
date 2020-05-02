using System;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.UseCase.InGame.Dto;
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
        private readonly Func<ResultDto, UniTask> m_onResult;

        private readonly Func<PuttedDto, UniTask> m_onPut;

        private readonly IGameResultService m_gameResultService;

        public PuttedStoneEventSubscriber(Func<ResultDto, UniTask> onResult, Func<PuttedDto, UniTask> onPut, IGameResultService gameResultService)
        {
            m_onResult = onResult;
            m_onPut = onPut;
            m_gameResultService = gameResultService;
        }

        public async UniTask ReceivedMessage<EventMessage>(EventMessage message) where EventMessage : IEventMessage
        {
            if (!(message is PuttedStoneEvent puttedStoneEvent)) throw new System.AggregateException();

            var game = puttedStoneEvent.Game;
            if (game.State == State.GameSet)
            {
                var result = await m_gameResultService.FindById(game.ResultId);
                _ = UniTask.WhenAll(
                    m_onResult(result),
                    m_onPut(new PuttedDto(game.Stones))
                    );
                return;
            }

            _ = m_onPut(new PuttedDto(game.Stones));
        }
    }
}
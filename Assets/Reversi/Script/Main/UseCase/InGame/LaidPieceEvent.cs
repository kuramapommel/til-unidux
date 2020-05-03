using System;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.UseCase.InGame.Dto;
using Pommel.Reversi.UseCase.Shared;
using UniRx.Async;

namespace Pommel.Reversi.UseCase.InGame
{
    public interface ILaidPieceEvent : IEventMessage
    {
        IGame Game { get; }
    }

    public sealed class LaidPieceEvent : ILaidPieceEvent
    {
        public IGame Game { get; }

        public LaidPieceEvent(IGame game) => Game = game;
    }

    public sealed class LaidPieceEventSubscriber : IEventSubscriber
    {
        private readonly Func<ResultDto, UniTask> m_onResult;

        private readonly Func<LaidDto, UniTask> m_onLaid;

        private readonly IGameResultService m_gameResultService;

        public LaidPieceEventSubscriber(Func<ResultDto, UniTask> onResult, Func<LaidDto, UniTask> onLaid, IGameResultService gameResultService)
        {
            m_onResult = onResult;
            m_onLaid = onLaid;
            m_gameResultService = gameResultService;
        }

        public async UniTask ReceivedMessage<EventMessage>(EventMessage message) where EventMessage : IEventMessage
        {
            if (!(message is ILaidPieceEvent laidPieceEvent)) throw new AggregateException();

            var game = laidPieceEvent.Game;
            if (game.State == State.GameSet)
            {
                var result = await m_gameResultService.FindById(game.ResultId);
                _ = UniTask.WhenAll(
                    m_onResult(result),
                    m_onLaid(new LaidDto(game.Pieces))
                    );
                return;
            }

            _ = m_onLaid(new LaidDto(game.Pieces));
        }
    }
}
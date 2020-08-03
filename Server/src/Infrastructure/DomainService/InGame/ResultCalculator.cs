using System;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using Pommel.Server.Component.Reactive;
using Pommel.Server.Domain;
using Pommel.Server.Domain.InGame;
using Pommel.Server.Infrastructure.Store.InGame;
using Pommel.Server.UseCase.InGame.Message;
using static LanguageExt.Prelude;
using _State = Pommel.Server.Domain.InGame.State;

namespace Pommel.Server.Infrastructure.DomainService.InGame
{
    public sealed class ResultCalculator : IResultCalculator
    {
        private readonly IMessagePublisher<IResultMessage> m_resultMessagePublisher;

        private readonly IGameResultStore m_resultStore;

        private readonly IGameResultFactory m_gameResultFactory;

        public ResultCalculator(
            IMessageBroker<IResultMessage> messageBroker,
            IGameResultStore resultStore,
            IGameResultFactory gameResultFactory
            )
        {
            m_resultMessagePublisher = messageBroker;
            m_resultStore = resultStore;
            m_gameResultFactory = gameResultFactory;
        }

        public async Task<Either<IError, IGameResult>> Calculate(IGame game)
        {
            await Task.CompletedTask;
            if (game.State != _State.GameSet) return Left<IError, IGameResult>(new DomainError(new ArgumentException(), "終了していないゲームです"));

            // ゲーム終了時のリザルト処理を行っているが、
            // 実際の運用だとゲーム結果を保存する api でサーバ側で勝手にやってくれる想定
            var darkCount = game.Pieces.Count(piece => piece.Color == Color.Dark);
            var lightCount = game.Pieces.Count(piece => piece.Color == Color.Light);
            var winner = darkCount == lightCount
                ? Winner.Draw
                : darkCount > lightCount
                ? Winner.Black
                : Winner.White;

            var result = m_gameResultFactory.Create(game.ResultId, darkCount, lightCount, winner);
            m_resultStore.Add(game.ResultId, result);
            m_resultMessagePublisher.Publish(new ResultMessage(game.ResultId));

            return Right<IError, IGameResult>(result);
        }
    }
}
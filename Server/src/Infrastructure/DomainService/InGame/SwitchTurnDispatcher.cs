using System;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using Pommel.Server.Domain;
using Pommel.Server.Domain.InGame;
using Pommel.Server.Infrastructure.Store.InGame;
using Pommel.Server.UseCase.InGame.Dto;
using static LanguageExt.Prelude;

namespace Pommel.Server.Infrastructure.DomainService.InGame
{
    public sealed class SwitchTurnDispatcher : ISwitchTurnDispatcher
    {
        private readonly IMatchingStore m_matchingStore;

        private readonly ILaidResultStore m_laidResultStore;

        public SwitchTurnDispatcher(
            IMatchingStore matchingStore,
            ILaidResultStore laidResultStore
            )
        {
            m_matchingStore = matchingStore;
            m_laidResultStore = laidResultStore;
        }

        public async Task<Either<IError, IGame>> Dispatch(IGame game)
        {
            if (!m_matchingStore.TryGetValue(game.MatchingId, out var matching))
                return Left<IError, IGame>(new DomainError(new ArgumentOutOfRangeException(), "存在しないマッチングが指定されました"));

            var nextPlayer = game.Turn == Turn.First
                ? matching.FirstPlayer
                : matching.SecondPlayer;

            var laidDto = new LaidDto(game.Pieces, nextPlayer);
            var logId = game.HistoryIds.LastOrDefault();
            if (!logId.IsNull()) m_laidResultStore.Add(logId, laidDto);

            await Task.CompletedTask;
            return Right<IError, IGame>(game);
        }
    }
}
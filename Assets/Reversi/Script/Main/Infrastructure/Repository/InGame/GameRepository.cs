using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using Pommel.Reversi.Domain;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Infrastructure.Networking.Client;
using Pommel.Reversi.Infrastructure.Store.InGame;
using Pommel.Reversi.UseCase.InGame.Dto;
using static LanguageExt.Prelude;
using _State = Pommel.Reversi.Domain.InGame.State;

namespace Pommel.Reversi.Infrastructure.Repository.InGame
{
    public sealed class GameRepository : IGameRepository
    {
        private readonly IGameStore m_store;

        private readonly IGameResultStore m_resultStore;

        private readonly ILaidResultStore m_laidResultStore;

        private readonly IMatchingStore m_matchingStore;

        private readonly IInGameClientFactory m_inGameClientFatory;

        private readonly IDictionary<bool, IInGameClient> m_inGameClientContainer = new Dictionary<bool, IInGameClient>();

        public GameRepository(
            IGameStore store,
            IGameResultStore resultStore,
            ILaidResultStore laidResultStore,
            IMatchingStore matchingStore,
            IInGameClientFactory inGameClientFactory
            )
        {
            m_store = store;
            m_resultStore = resultStore;
            m_laidResultStore = laidResultStore;
            m_matchingStore = matchingStore;
            m_inGameClientFatory = inGameClientFactory;
        }

        public Task<Either<IError, IGame>> FindById(string id) => Task.FromResult(
            m_store.TryGetValue(id)
                .ToEither(new DomainError(new ArgumentOutOfRangeException(), "存在しないゲームIDが指定されました") as IError));

        public async Task<Either<IError, IGame>> Save(IGame game)
        {
            if (m_store.ContainsKey(game.Id)) m_store.Remove(game.Id);
            m_store.Add(game.Id, game);

            var inGameClient = m_inGameClientContainer.TryGetValue(true, out var client)
                ? client
                : m_inGameClientFatory.Create();

            _ = inGameClient.SaveAsync(game);
            if (!m_inGameClientContainer.ContainsKey(true)) m_inGameClientContainer.Add(true, inGameClient);

            if (!m_matchingStore.TryGetValue(game.MatchingId, out var matching))
                return Left<IError, IGame>(new DomainError(new ArgumentOutOfRangeException(), "存在しないマッチングが指定されました"));

            var nextPlayer = game.Turn == Turn.First
                ? matching.FirstPlayer
                : matching.SecondPlayer;

            var laidDto = new LaidDto(game.Pieces, nextPlayer);
            var logId = game.HistoryIds.LastOrDefault();
            if (!logId.IsNull()) m_laidResultStore.Add(logId, laidDto);

            if (game.State != _State.GameSet) return await FindById(game.Id);

            // ゲーム終了時のリザルト処理を行っているが、
            // 実際の運用だとゲーム結果を保存する api でサーバ側で勝手にやってくれる想定
            var darkCount = game.Pieces.Count(piece => piece.Color == Color.Dark);
            var lightCount = game.Pieces.Count(piece => piece.Color == Color.Light);
            var winner = darkCount == lightCount
                ? Winner.Draw
                : darkCount > lightCount
                ? Winner.Black
                : Winner.White;

            var dto = new ResultDto(game.ResultId, (dark: darkCount, light: lightCount), winner);
            m_resultStore.Add(game.ResultId, dto);

            return await FindById(game.Id);
        }

        public Task<Either<IError, IEnumerable<IGame>>> Fetch(Func<IGame, bool> predicate)
        {
            var games = m_store.Values.Where(predicate);
            return Task.FromResult(
                games.Any()
                ? Right<IError, IEnumerable<IGame>>(games)
                : Left<IError, IEnumerable<IGame>>(new DomainError(new ArgumentOutOfRangeException(), "該当のゲームが存在しません")));
        }
    }
}
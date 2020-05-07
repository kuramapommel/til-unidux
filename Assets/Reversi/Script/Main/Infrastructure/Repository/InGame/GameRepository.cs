using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using Pommel.Reversi.Domain;
using Pommel.Reversi.Domain.InGame;
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

        public GameRepository(IGameStore store, IGameResultStore resultStore)
        {
            m_store = store;
            m_resultStore = resultStore;
        }

        public EitherAsync<IError, IGame> FindById(string id) => m_store.TryGetValue(id, out var game)
            ? RightAsync<IError, IGame>(game)
            : LeftAsync<IError, IGame>(new DomainError(new ArgumentOutOfRangeException(), "存在しないゲームIDが指定されました"));


        public EitherAsync<IError, IGame> Save(IGame game)
        {
            if (m_store.ContainsKey(game.Id)) m_store.Remove(game.Id);
            m_store.Add(game.Id, game);

            if (game.State != _State.GameSet) return FindById(game.Id);

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

            return FindById(game.Id);
        }

        public EitherAsync<IError, IEnumerable<IGame>> Fetch(Func<IGame, bool> predicate)
        {
            var games = m_store.Values.Where(predicate);
            return games.Any() ? RightAsync<IError, IEnumerable<IGame>>(games) : LeftAsync<IError, IEnumerable<IGame>>(new DomainError(new ArgumentOutOfRangeException(), "該当のゲームが存在しません"));
        }
    }
}
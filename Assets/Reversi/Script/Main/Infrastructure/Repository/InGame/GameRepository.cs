using System;
using System.Collections.Generic;
using System.Linq;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Infrastructure.Store.InGame;
using Pommel.Reversi.UseCase.InGame.Dto;
using UniRx.Async;

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

        public UniTask<IGame> FindById(string id) => UniTask.Run(() => m_store[id]);

        public async UniTask<IGame> Save(IGame game)
        {
            var saveGameTask = UniTask.Run(() =>
            {
                if (m_store.ContainsKey(game.Id)) m_store.Remove(game.Id);
                m_store.Add(game.Id, game);
            });

            // ゲーム終了時のリザルト処理を行っているが、
            // 実際の運用だとゲーム結果を保存する api でサーバ側で勝手にやってくれる想定
            var saveResultTask = game.State == State.GameSet
                ? UniTask.Run(() =>
                {
                    var darkCount = game.Pieces.Count(piece => piece.Color == Color.Dark);
                    var lightCount = game.Pieces.Count(piece => piece.Color == Color.Light);
                    var winner = darkCount == lightCount
                        ? Winner.Draw
                        : darkCount > lightCount
                        ? Winner.Black
                        : Winner.White;

                    var dto = new ResultDto(game.ResultId, (dark: darkCount, light: lightCount), winner);
                    m_resultStore.Add(game.ResultId, dto);
                })
                : UniTask.CompletedTask;

            await UniTask.WhenAll(saveGameTask, saveResultTask);

            return m_store[game.Id];
        }

        public UniTask<IEnumerable<IGame>> Fetch(Func<IGame, bool> predicate) => UniTask.Run<IEnumerable<IGame>>(() => m_store.Values.Where(predicate).ToArray());
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using Pommel.Server.Domain;
using Pommel.Server.Domain.InGame;
using Pommel.Server.Infrastructure.Store.InGame;
using static LanguageExt.Prelude;

namespace Pommel.Server.Infrastructure.Repository.InGame
{
    public sealed class GameRepository : IGameRepository
    {
        private readonly IGameStore m_store;

        public GameRepository(
            IGameStore store
            )
        {
            m_store = store;
        }

        public Task<Either<IError, IGame>> FindById(string id) => Task.FromResult(
            m_store.TryGetValue(id)
                .ToEither(new DomainError(new ArgumentOutOfRangeException(), "存在しないゲームIDが指定されました") as IError));

        public async Task<Either<IError, IGame>> Save(IGame game)
        {
            lock(m_store)
            {
                if (m_store.ContainsKey(game.Id)) m_store.Remove(game.Id);
                m_store.Add(game.Id, game);
            }

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
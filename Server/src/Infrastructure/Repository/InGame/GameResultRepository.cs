using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using Pommel.Server.Domain;
using Pommel.Server.Domain.InGame;
using static LanguageExt.Prelude;

namespace Pommel.Server.Infrastructure.Repository.InGame
{
    public sealed class GameResultRepository : IResultRepository
    {
        private readonly IDictionary<string, IGameResult> m_store = new Dictionary<string, IGameResult>();
        
        public Task<Either<IError, IGameResult>> FindById(string id) => Task.FromResult(
            m_store.TryGetValue(id)
                .ToEither(new DomainError(new ArgumentOutOfRangeException(), "存在しないゲームIDが指定されました") as IError));

        public async Task<Either<IError, IGameResult>> Save(IGameResult gameResult)
        {
            lock(m_store)
            {
                if (m_store.ContainsKey(gameResult.Id)) m_store.Remove(gameResult.Id);
                m_store.Add(gameResult.Id, gameResult);
            }

            return await FindById(gameResult.Id);
        }

        public Task<Either<IError, IEnumerable<IGameResult>>> Fetch(Func<IGameResult, bool> predicate)
        {
            var matching = m_store.Values.Where(predicate);
            return Task.FromResult(
                matching.Any()
                ? Right<IError, IEnumerable<IGameResult>>(matching)
                : Left<IError, IEnumerable<IGameResult>>(new DomainError(new ArgumentOutOfRangeException(), "該当のゲームが存在しません")));
        }
    }
}
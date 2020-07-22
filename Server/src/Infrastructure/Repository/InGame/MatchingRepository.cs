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
    public sealed class MatchingRepository : IMatchingRepository
    {
        private readonly IMatchingStore m_store;

        public MatchingRepository(IMatchingStore store) => m_store = store;

        public Task<Either<IError, IMatching>> FindById(string id) => Task.FromResult(
            m_store.TryGetValue(id)
                .ToEither(new DomainError(new ArgumentOutOfRangeException(), $"存在しないマッチングが指定されました. 指定されたマッチングID = {id}") as IError));

        public async Task<Either<IError, IMatching>> Save(IMatching matching)
        {
            if (m_store.ContainsKey(matching.Id)) m_store.Remove(matching.Id);
            m_store.Add(matching.Id, matching);

            return await FindById(matching.Id);
        }

        public Task<Either<IError, IEnumerable<IMatching>>> Fetch(Func<IMatching, bool> predicate)
        {
            var matching = m_store.Values.Where(predicate);
            return Task.FromResult(
                matching.Any()
                ? Right<IError, IEnumerable<IMatching>>(matching)
                : Left<IError, IEnumerable<IMatching>>(new DomainError(new ArgumentOutOfRangeException(), "該当のマッチングが存在しません")));
        }
    }
}
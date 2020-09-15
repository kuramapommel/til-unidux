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
    public sealed class RoomRepository : IRoomRepository
    {
        private readonly IRoomStore m_store;

        public RoomRepository(IRoomStore store) => m_store = store;

        public Task<Either<IError, IRoom>> FindById(string id) => Task.FromResult(
            m_store.TryGetValue(id)
                .ToEither(new DomainError(new ArgumentOutOfRangeException(), $"存在しないマッチングが指定されました. 指定されたマッチングID = {id}") as IError));

        public async Task<Either<IError, IRoom>> Save(IRoom room)
        {
            lock(m_store)
            {
                if (m_store.ContainsKey(room.Id)) m_store.Remove(room.Id);
                m_store.Add(room.Id, room);
            }

            return await FindById(room.Id);
        }

        public Task<Either<IError, IEnumerable<IRoom>>> Fetch(Func<IRoom, bool> predicate)
        {
            var room = m_store.Values.Where(predicate);
            return Task.FromResult(
                room.Any()
                ? Right<IError, IEnumerable<IRoom>>(room)
                : Left<IError, IEnumerable<IRoom>>(new DomainError(new ArgumentOutOfRangeException(), "該当のマッチングが存在しません")));
        }
    }
}
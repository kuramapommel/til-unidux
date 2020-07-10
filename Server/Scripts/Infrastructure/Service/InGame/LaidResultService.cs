using System.Threading.Tasks;
using Pommel.Server.Infrastructure.Store.InGame;
using Pommel.Server.UseCase.InGame;
using Pommel.Server.UseCase.InGame.Dto;

namespace Pommel.Server.Infrastructure.Service.InGame
{
    public sealed class LaidResultService : ILaidResultService
    {
        private readonly ILaidResultStore m_store;

        public LaidResultService(ILaidResultStore store) => m_store = store;

        public async Task<LaidDto> FindById(string id)
        {
            await Task.CompletedTask;
            return m_store[id];
        }
    }
}
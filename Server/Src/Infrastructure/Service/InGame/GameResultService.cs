using System.Threading.Tasks;
using Pommel.Server.Infrastructure.Store.InGame;
using Pommel.Server.UseCase.InGame;
using Pommel.Server.UseCase.InGame.Dto;

namespace Pommel.Server.Infrastructure.Service.InGame
{
    public sealed class GameResultService : IGameResultService
    {
        private readonly IGameResultStore m_store;

        public GameResultService(IGameResultStore store) => m_store = store;

        public async Task<ResultDto> FindById(string id)
        {
            await Task.CompletedTask;
            return m_store[id];
        }
    }
}
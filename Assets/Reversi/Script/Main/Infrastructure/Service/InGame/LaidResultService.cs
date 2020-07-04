using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Pommel.Reversi.Infrastructure.Store.InGame;
using Pommel.Reversi.UseCase.InGame;
using Pommel.Reversi.UseCase.InGame.Dto;

namespace Pommel.Reversi.Infrastructure.Service.InGame
{
    public sealed class LaidResultService : ILaidResultService
    {
        private readonly ILaidResultStore m_store;

        public LaidResultService(ILaidResultStore store) => m_store = store;

        public async Task<LaidDto> FindById(string id)
        {
            await UniTask.CompletedTask;
            return m_store[id];
        }
    }
}
using Cysharp.Threading.Tasks;
using Pommel.Reversi.Infrastructure.Store.InGame;
using Pommel.Reversi.UseCase.InGame;
using Pommel.Reversi.UseCase.InGame.Dto;

namespace Pommel.Reversi.Infrastructure.Service.InGame
{
    public sealed class GameResultService : IGameResultService
    {
        private readonly IGameResultStore m_store;

        public GameResultService(IGameResultStore store) => m_store = store;

        public UniTask<ResultDto> FindById(string id) => UniTask.Run(() => m_store[id]);
    }
}
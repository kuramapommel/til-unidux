using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Infrastructure.Store.InGame;
using UniRx.Async;

namespace Pommel.Reversi.Infrastructure.Repository.InGame
{
    public sealed class GameRepository : IGameRepository
    {
        private readonly IGameStore m_store;

        public GameRepository(IGameStore store) => m_store = store;

        public UniTask<IGame> FindById(string id) => UniTask.Run(() => m_store[id]);

        public UniTask<IGame> Save(IGame game) => UniTask.Run(() =>
        {
            var key = game.Id;
            if (m_store.ContainsKey(key)) m_store.Remove(key);
            m_store.Add(key, game);
            return m_store[key];
        });
    }
}
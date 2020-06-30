using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Infrastructure.Store.InGame;
using Pommel.Reversi.UseCase.InGame;

namespace Pommel.Reversi.Infrastructure.Service.InGame
{
    public sealed class GameService : IGameService
    {
        private readonly IGameStore m_store;

        public GameService(IGameStore store) => m_store = store;

        public UniTask<IGame> FetchPlaying() => UniTask.Run(() =>
            m_store.Values.FirstOrDefault(game => game.State == State.Playing) ?? throw new IndexOutOfRangeException());
    }
}
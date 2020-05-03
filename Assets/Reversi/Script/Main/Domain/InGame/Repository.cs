using UniRx.Async;
using System.Collections.Generic;
using System;

namespace Pommel.Reversi.Domain.InGame
{
    // 試合集約のrepository
    public interface IGameRepository
    {
        UniTask<IGame> FindById(string id);

        UniTask<IGame> Save(IGame game);

        UniTask<IEnumerable<IGame>> Fetch(Func<IGame, bool> predicate);
    }
}
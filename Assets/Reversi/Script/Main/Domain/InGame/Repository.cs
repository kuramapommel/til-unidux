using System;
using System.Collections.Generic;
using LanguageExt;

namespace Pommel.Reversi.Domain.InGame
{
    // 試合集約のrepository
    public interface IGameRepository
    {
        EitherAsync<IError, IGame> FindById(string id);

        EitherAsync<IError, IGame> Save(IGame game);

        EitherAsync<IError, IEnumerable<IGame>> Fetch(Func<IGame, bool> predicate);
    }
}
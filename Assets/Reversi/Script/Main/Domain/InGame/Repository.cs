using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;

namespace Pommel.Reversi.Domain.InGame
{
    // 試合集約のrepository
    public interface IGameRepository
    {
        Task<Either<IError, IGame>> FindById(string id);

        Task<Either<IError, IGame>> Save(IGame game);

        Task<Either<IError, IEnumerable<IGame>>> Fetch(Func<IGame, bool> predicate);
    }
}
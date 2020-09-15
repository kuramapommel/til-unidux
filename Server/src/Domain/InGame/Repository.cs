using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;

namespace Pommel.Server.Domain.InGame
{
    // 試合集約のrepository
    public interface IGameRepository
    {
        Task<Either<IError, IGame>> FindById(string id);

        Task<Either<IError, IGame>> Save(IGame game);

        Task<Either<IError, IEnumerable<IGame>>> Fetch(Func<IGame, bool> predicate);
    }

    public interface IMatchingRepository
    {
        Task<Either<IError, IMatching>> FindById(string id);

        Task<Either<IError, IMatching>> Save(IMatching matching);

        Task<Either<IError, IEnumerable<IMatching>>> Fetch(Func<IMatching, bool> predicate);
    }

    public interface IResultRepository
    {
        Task<Either<IError, IGameResult>> FindById(string id);

        Task<Either<IError, IGameResult>> Save(IGameResult gameResult);

        Task<Either<IError, IEnumerable<IGameResult>>> Fetch(Func<IGameResult, bool> predicate);
    }

    public interface IRoomRepository
    {
        Task<Either<IError, IRoom>> FindById(string id);

        Task<Either<IError, IRoom>> Save(IRoom room);

        Task<Either<IError, IEnumerable<IRoom>>> Fetch(Func<IRoom, bool> predicate);
    }
}
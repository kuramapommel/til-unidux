using System.Threading.Tasks;
using LanguageExt;

namespace Pommel.Server.Domain.InGame
{
    public interface IResultCalculator
    {
        Task<Either<IError, IGameResult>> Calculate(IGame game);
    }
}
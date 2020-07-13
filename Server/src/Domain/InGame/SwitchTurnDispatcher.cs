using System.Threading.Tasks;
using LanguageExt;

namespace Pommel.Server.Domain.InGame
{
    public interface ISwitchTurnDispatcher
    {
        Task<Either<IError, IGame>> Dispatch(IGame game);
    }
}
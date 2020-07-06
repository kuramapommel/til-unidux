using MagicOnion;
using Pommel.Api.Protocol.InGame;

namespace Pommel.Api.Services
{
    public interface IInGameService : IService<IInGameService>
    {
        UnaryResult<Game> SaveGameAsync(Game game);
    }
}
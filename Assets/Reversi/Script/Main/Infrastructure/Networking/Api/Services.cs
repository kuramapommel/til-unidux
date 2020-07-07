using MagicOnion;
using Pommel.Api.Protocol.InGame;

namespace Pommel.Api.Services
{
    public interface IInGameService : IService<IInGameService>
    {
        UnaryResult<string> CreateMatchingAsync(string playerId, string playerName);

        UnaryResult<string> CreateGameAsync(string matchingId);

        UnaryResult<Game> SaveGameAsync(Game game);
    }
}
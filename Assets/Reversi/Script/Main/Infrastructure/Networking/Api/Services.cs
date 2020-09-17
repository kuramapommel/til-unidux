using MagicOnion;
using Pommel.Api.Protocol.InGame;

namespace Pommel.Api.Services
{
    public interface IInGameService : IService<IInGameService>
    {
        UnaryResult<string> CreateGameAsync();

        UnaryResult<string> EntryRoomAsync(string roomId, string playerId, string playerName);

        UnaryResult<Game> FindGameById(string gameId);
    }
}
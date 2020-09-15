using MagicOnion;
using Pommel.Api.Protocol.InGame;

namespace Pommel.Api.Services
{
    public interface IInGameService : IService<IInGameService>
    {
        UnaryResult<string> CreateRoomAsync();

        UnaryResult<string> CreateGameAsync(string roomId);

        UnaryResult<string> EntryRoomAsync(string roomId, string playerId, string playerName);

        UnaryResult<Room> FindRoomById(string roomId);
    }
}
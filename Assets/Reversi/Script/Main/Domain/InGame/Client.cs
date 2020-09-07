using System.Threading.Tasks;

namespace Pommel.Reversi.Domain.InGame
{
    public interface IClient
    {
        Task ConnectAsync();

        Task DisconnectAsync();

        Task PutStoneAsync(int x, int y);

        Task CreateGameAsync(string roomId);

        Task<string> CreateRoomAsync();

        Task<string> EntryRoomAsync(string roomId, string playerId, string playerName);

        Task<ValueObjects.Room> FindRoomById(string roomId);
    }
}
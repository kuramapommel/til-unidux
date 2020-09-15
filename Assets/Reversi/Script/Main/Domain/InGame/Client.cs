using System.Threading.Tasks;

namespace Pommel.Reversi.Domain.InGame
{
    public interface IClient
    {
        Task ConnectAsync();

        Task DisconnectAsync();

        Task PutStoneAsync(int x, int y);

        Task EntryRoomAsync(string roomId, string playerId, string playerName);

        Task StartGameAsync(string gameId);

        Task<string> CreateGameAsync(string roomId);

        Task<string> CreateRoomAsync();

        Task<ValueObjects.Room> FindRoomById(string roomId);
    }
}
using System.Threading.Tasks;
using MagicOnion;
using Pommel.Api.Protocol.InGame;

namespace Pommel.Api.Hubs
{
    public interface IInGameHub : IStreamingHub<IInGameHub, IInGameReceiver>
    {
        Task CreateMatchingAsync(string playerId, string playerName);

        Task EntryMatchingAsync(string matchingId, string playerId, string playerName);

        Task CreateGameAsync(string matchingId);

        Task LayAsync(string gameId, int x, int y);
    }

    public interface IInGameReceiver
    {
        void OnJoin(string matchingId, string player1Id, string player1Name, string player2Id, string player2Name);

        void OnStartGame(string nextPlayerId, string matchingId, Game game);

        void OnLay(string nextPlayerId, Game game);

        void OnResult(int darkCount, int lightCount, int winner);
    }
}
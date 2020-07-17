using System.Threading.Tasks;
using MagicOnion;
using Pommel.Api.Protocol.InGame;

namespace Pommel.Api.Hubs
{
    public interface IInGameHub : IStreamingHub<IInGameHub, IInGameReceiver>
    {
        Task JoinAsync(string matchingId, string playerId, string playerName);

        Task CreateMatchingAsync(string playerId, string playerName);

        Task EntryMatchingAsync(string matchingId, string playerId, string playerName);

        Task CreateGameAsync(string matchingId);

        Task StartGameAsync(string gameId);

        Task LayAsync(string gameId, int x, int y);
    }

    public interface IInGameReceiver
    {
        void OnJoin(string matchingId, string playerId, string playerName);

        void OnCreateGame(string gameId, string matchingId);

        void OnStartGame(string nextPlayerId, Game game);

        void OnLay(string nextPlayerId, Game game);

        void OnResult(int darkCount, int lightCount, int winner);
    }
}
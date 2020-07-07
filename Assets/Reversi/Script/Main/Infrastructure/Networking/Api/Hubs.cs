using System.Threading.Tasks;
using MagicOnion;
using Pommel.Api.Protocol.InGame;

namespace Pommel.Api.Hubs
{
    public interface IInGameHub : IStreamingHub<IInGameHub, IInGameReceiver>
    {
        Task JoinAsync(string matchingId, string playerId, string playerName);

        Task StartGameAsync(string gameId);

        Task LayAsync(string gameId, int x, int y);
    }

    public interface IInGameReceiver
    {
        void OnCreateMatching(string matchingId);

        void OnJoin(string matchingId, string playerId, string playerName);

        void OnCreateGame(string gameId, string matchingId);

        void OnStartGame(Game game);

        void OnLay(Game game);

        void OnResult(int darkCount, int lightCount, int winner);
    }
}
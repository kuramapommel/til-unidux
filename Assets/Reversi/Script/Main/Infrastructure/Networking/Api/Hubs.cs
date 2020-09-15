using System.Threading.Tasks;
using MagicOnion;
using Pommel.Api.Protocol.InGame;

namespace Pommel.Api.Hubs
{
    public interface IInGameHub : IStreamingHub<IInGameHub, IInGameReceiver>
    {
        Task EntryRoomAsync(string roomId, string playerId, string playerName);

        Task StartGameAsync(string gameId);

        Task LayAsync(int x, int y);
    }

    public interface IInGameReceiver
    {
        void OnRefresh(Game game);

        void OnStart(Game game);
    }
}
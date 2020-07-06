using System.Threading.Tasks;
using MagicOnion.Server.Hubs;
using Pommel.Api.Hubs;

namespace Pommel.Server.Controller.Hub
{
    public sealed class InGameHub : StreamingHubBase<IInGameHub, IInGameReceiver>, IInGameHub
    {
        private const string ROOM_NAME = "sample";

        IGroup room;

        public async Task JoinAsync()
        {
            Logger.Debug($"joined");
            room = await Group.AddAsync(ROOM_NAME);
        }

        public async Task LayAsync(string gameId, int x, int y)
        {
            Logger.Debug($"game id is {gameId}, lay point x = {x}, y = {y}");
            BroadcastExceptSelf(room).OnLay(gameId, x, y);
            await Task.CompletedTask;
        }
    }
}
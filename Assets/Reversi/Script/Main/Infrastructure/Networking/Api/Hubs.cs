using System.Threading.Tasks;
using MagicOnion;

namespace Pommel.Api.Hubs
{
    public interface IInGameHub : IStreamingHub<IInGameHub, IInGameReceiver>
    {
        Task JoinAsync();

        Task LayAsync(string gameId, int x, int y);
    }

    public interface IInGameReceiver
    {
        void OnLay(string gameId, int x, int y);
    }
}
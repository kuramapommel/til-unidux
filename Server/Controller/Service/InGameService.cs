using System.Threading.Tasks;
using MagicOnion;
using MagicOnion.Server;
using Pommel.Api.Protocol.InGame;
using Pommel.Api.Services;

namespace Pommel.Server.Controller.Service
{
    public sealed class InGameService : ServiceBase<IInGameService>, IInGameService
    {
        public async UnaryResult<Game> SaveGameAsync(Game game)
        {
            Logger.Debug($"game id is {game.Id}");
            foreach (var piece in game.Pieces)
                Logger.Debug($"piece x is {piece.X}, piece y is {piece.Y}");

            await Task.CompletedTask;
            return game;
        }
    }
}
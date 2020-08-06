using System.Threading.Tasks;
using Pommel.Reversi.Infrastructure.Networking.Client;
using System;

namespace Pommel.Reversi.Presentation.Model.InGame
{
    public interface IPieceModel : IDisposable
    {
        Task LayPiece(string gameId, int x, int y);
    }
        
    public sealed class PieceModel : IPieceModel
    {
        private readonly IInGameClient m_client;

        public PieceModel(
            IInGameClient inGameClient
            )
        {
            m_client = inGameClient;
        }

        public async Task LayPiece(string gameId, int x, int y) =>
            await m_client.LayAsync(gameId, x, y);

        void IDisposable.Dispose()
        {
        }
    }
}
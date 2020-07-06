using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.UseCase.InGame;

namespace Pommel.Reversi.Presentation.Model.InGame
{
    public interface IPieceModel
    {
        Task<IGame> LayPiece(string gameId, int x, int y);
    }
        
    public sealed class PieceModel : IPieceModel
    {
        private readonly ILayPieceUseCase m_layPieceUseCase;

        public PieceModel(ILayPieceUseCase layPieceUseCase)
        {
            m_layPieceUseCase = layPieceUseCase;
        }

        public async Task<IGame> LayPiece(string gameId, int x, int y) => await m_layPieceUseCase.Execute(gameId, x, y)
            .Match(
                Right: game => game,
                // todo error handling
                Left: error => throw error.Exception
            )
            .AsUniTask();
    }
}
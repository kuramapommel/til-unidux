using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.UseCase.InGame;
using UniRx;

namespace Pommel.Reversi.Presentation.Model.InGame
{
    public interface IPieceModel
    {
        Task<IGame> LayPiece(string gameId, int x, int y);
    }

    public sealed class PieceModel : IPieceModel
    {
        private readonly ILayPieceUseCase m_layPieceUseCase;

        private readonly IMessagePublisher m_messagePublisher;

        public PieceModel(ILayPieceUseCase layPieceUseCase, IMessageBroker messagePublisher)
        {
            m_layPieceUseCase = layPieceUseCase;
            m_messagePublisher = messagePublisher;
        }

        public async Task<IGame> LayPiece(string gameId, int x, int y) => await m_layPieceUseCase.Execute(gameId, x, y)
            .Match(
                Right: game =>
                {
                    m_messagePublisher.Publish<ILaidPieceEvent>(new LaidPieceEvent(game));
                    return game;
                },
                // todo error handling
                Left: error => throw error.Exception
            )
            .AsUniTask();
    }
}
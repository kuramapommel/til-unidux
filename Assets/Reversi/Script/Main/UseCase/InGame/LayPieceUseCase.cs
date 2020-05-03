using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.UseCase.Shared;
using UniRx.Async;

namespace Pommel.Reversi.UseCase.InGame
{
    public interface ILayPieceUseCase
    {
        UniTask<IGame> Execute(int x, int y);
    }

    public sealed class LayPieceUseCase : ILayPieceUseCase
    {
        private readonly IGameRepository m_gameRepository;

        private readonly IEventPublisher m_publisher;

        private readonly string m_gameId;

        public LayPieceUseCase(IGameRepository gameRepository, IEventPublisher publisher, string gameId)
        {
            m_gameRepository = gameRepository;
            m_publisher = publisher;
            m_gameId = gameId;
        }

        public async UniTask<IGame> Execute(int x, int y)
        {
            var point = new Point(x, y);
            var game = await m_gameRepository.FindById(m_gameId);
            var putted = game.LayPiece(point);
            var savedGame = await m_gameRepository.Save(putted);
            _ = m_publisher.Publish<ILaidPieceEvent>(new LaidPieceEvent(savedGame));
            return savedGame;
        }
    }
}
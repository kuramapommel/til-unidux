using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.UseCase.Shared;
using UniRx.Async;

namespace Pommel.Reversi.UseCase.InGame
{
    public interface IPutStoneUseCase
    {

    }

    public sealed class PutStoneUseCase : IPutStoneUseCase
    {
        private readonly IGameRepository m_gameRepository;

        private readonly IEventPublisher m_publisher;

        private readonly string m_gameId;

        private readonly Point m_point;

        public PutStoneUseCase(IGameRepository gameRepository, IEventPublisher publisher, string gameId, int x, int y)
        {
            m_gameRepository = gameRepository;
            m_publisher = publisher;
            m_gameId = gameId;
            m_point = new Point(x, y);
        }

        public async UniTask<IGame> Execute()
        {
            var game = await m_gameRepository.FindById(m_gameId);
            var putted = game.PutStone(m_point);
            var savedGame = await m_gameRepository.Save(putted);
            _ = m_publisher.Publish(new PuttedStoneEvent());
            return savedGame;
        }
    }
}
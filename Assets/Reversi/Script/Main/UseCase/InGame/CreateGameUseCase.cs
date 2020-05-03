using Pommel.Reversi.Domain.InGame;
using UniRx.Async;
using Zenject;

namespace Pommel.Reversi.UseCase.InGame
{
    public interface ICreateGameUseCase
    {
        UniTask<IGame> Execute();
    }

    public sealed class CreateGameUseCase : ICreateGameUseCase
    {
        private readonly IGameRepository m_gameRepository;

        private readonly IFactory<string, IGame> m_gameFactory;

        public CreateGameUseCase(IGameRepository gameRepository, IFactory<string, IGame> gameFactory)
        {
            m_gameRepository = gameRepository;
            m_gameFactory = gameFactory;
        }

        public async UniTask<IGame> Execute()
        {
            // todo ID Generator 的なものをかませる
            var gameId = System.Guid.NewGuid().ToString();

            return await m_gameRepository.Save(m_gameFactory.Create(gameId));
        }
    }
}
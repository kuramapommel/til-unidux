using System;
using Pommel.Reversi.Domain;
using Pommel.Reversi.Domain.InGame;
using UniRx.Async;
using Zenject;
using static LanguageExt.Prelude;

namespace Pommel.Reversi.UseCase.InGame
{
    public interface ICreateGameUseCase
    {
        UniTask<IGame> Execute();
    }

    public sealed class CreateGameUseCase : ICreateGameUseCase
    {
        private readonly IGameRepository m_gameRepository;

        private readonly IFactory<string, string, IGame> m_gameFactory;

        public CreateGameUseCase(IGameRepository gameRepository, IFactory<string, string, IGame> gameFactory)
        {
            m_gameRepository = gameRepository;
            m_gameFactory = gameFactory;
        }

        public async UniTask<IGame> Execute() =>
            await match(
                from gameId in RightAsync<IError, string>(Guid.NewGuid().ToString()) // todo ID Generator 的なものをかませる
                from resultId in RightAsync<IError, string>(Guid.NewGuid().ToString()) // todo ID Generator 的なものをかませる
                from saved in m_gameRepository.Save(m_gameFactory.Create(gameId, resultId)).ToAsync()
                select saved,
                Right: game => game,
                Left: error => throw error.Exception
                );
    }
}
using System;
using LanguageExt;
using Pommel.Server.Domain;
using Pommel.Server.Domain.InGame;
using static LanguageExt.Prelude;

namespace Pommel.Server.UseCase.InGame
{
    public interface ICreateGameUseCase
    {
        EitherAsync<IError, IGame> Execute(string roomId);
    }

    public sealed class CreateGameUseCase : ICreateGameUseCase
    {
        private readonly IGameFactory m_gameFactory;

        private readonly IGameRepository m_gameRepository;

        public CreateGameUseCase(
            IGameFactory gameFactory,
            IGameRepository gameRepository
            )
        {
            m_gameFactory = gameFactory;
            m_gameRepository = gameRepository;
        }

        public EitherAsync<IError, IGame> Execute(string roomId) =>
                from gameId in RightAsync<IError, string>(Guid.NewGuid().ToString("N")) // todo ID Generator 的なものをかませる
                from resultId in RightAsync<IError, string>(Guid.NewGuid().ToString("N")) // todo ID Generator 的なものをかませる
                from game in Try(() => m_gameFactory.Create(gameId, resultId, roomId))
                    .ToEitherAsync()
                    .MapLeft(e => new DomainError(e) as IError)
                from savedGame in m_gameRepository.Save(game).ToAsync()
                select savedGame;

    }
}
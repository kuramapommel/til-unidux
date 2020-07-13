using System;
using LanguageExt;
using Pommel.Server.Domain;
using Pommel.Server.Domain.InGame;
using static LanguageExt.Prelude;

namespace Pommel.Server.UseCase.InGame
{
    public interface ICreateGameUseCase
    {
        EitherAsync<IError, IGame> Execute(string matchingId);
    }

    public sealed class CreateGameUseCase : ICreateGameUseCase
    {
        private readonly IGameRepository m_gameRepository;

        private readonly IMatchingRepository m_matchingRepository;

        private readonly IGameFactory m_gameFactory;

        public CreateGameUseCase(
            IGameRepository gameRepository,
            IMatchingRepository matchingRepository,
            IGameFactory gameFactory
            )
        {
            m_gameRepository = gameRepository;
            m_matchingRepository = matchingRepository;
            m_gameFactory = gameFactory;
        }

        public EitherAsync<IError, IGame> Execute(string matchingId) =>
            from matching in m_matchingRepository.FindById(matchingId).ToAsync()
            from gameId in RightAsync<IError, string>(Guid.NewGuid().ToString()) // todo ID Generator 的なものをかませる
            from resultId in RightAsync<IError, string>(Guid.NewGuid().ToString()) // todo ID Generator 的なものをかませる
            from game in Try(() => m_gameFactory.Create(gameId, resultId, matching.Id))
                .ToEitherAsync()
                .MapLeft(e => new DomainError(e) as IError)
            from saved in m_gameRepository.Save(game).ToAsync()
            select saved;
    }
}
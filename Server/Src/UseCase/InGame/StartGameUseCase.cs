using LanguageExt;
using Pommel.Server.Domain;
using Pommel.Server.Domain.InGame;
using static LanguageExt.Prelude;

namespace Pommel.Server.UseCase.InGame
{
    public interface IStartGameUseCase
    {
        EitherAsync<IError, IGame> Execute(string gameId);
    }

    public sealed class StartGameUseCase : IStartGameUseCase
    {
        private readonly IGameRepository m_gameRepository;

        private readonly IMatchingRepository m_matchingRepository;

        public StartGameUseCase(
            IGameRepository gameRepository,
            IMatchingRepository matchingRepository
            )
        {
            m_gameRepository = gameRepository;
            m_matchingRepository = matchingRepository;
        }

        public EitherAsync<IError, IGame> Execute(string gameId) =>
                from game in m_gameRepository.FindById(gameId).ToAsync()
                from firstPlayerId in m_matchingRepository.FindById(game.MatchingId).ToAsync()
                    .Map(matching => matching.FirstPlayer.Id)
                from started in Try(() => game.Start(firstPlayerId))
                    .ToEitherAsync()
                    .MapLeft(e => new DomainError(e) as IError)
                from saved in m_gameRepository.Save(started).ToAsync()
                select saved;
    }
}
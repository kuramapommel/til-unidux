using LanguageExt;
using Pommel.Reversi.Domain;
using Pommel.Reversi.Domain.InGame;
using static LanguageExt.Prelude;

namespace Pommel.Reversi.UseCase.InGame
{
    public interface IEntryMatchingUseCase
    {
        EitherAsync<IError, IMatching> Execute(string matchingId, string playerId, string playerName);
    }

    public sealed class EntryMatchingUseCase : IEntryMatchingUseCase
    {
        private readonly IPlayerFactory m_playerFactory;

        private readonly IMatchingRepository m_matchingRepository;

        public EntryMatchingUseCase(
            IPlayerFactory playerFactory,
            IMatchingRepository matchingRepository
            )
        {
            m_playerFactory = playerFactory;
            m_matchingRepository = matchingRepository;
        }

        public EitherAsync<IError, IMatching> Execute(string matchingId, string playerId, string playerName) =>
                from matching in m_matchingRepository.FindById(matchingId).ToAsync()
                from secondPlayer in Try(() => m_playerFactory.Create(playerId, playerName))
                    .ToEitherAsync()
                    .MapLeft(e => new DomainError(e) as IError)
                from entried in Try(() => matching.Entry(secondPlayer))
                    .ToEitherAsync()
                    .MapLeft(e => new DomainError(e) as IError)
                from savedMatching in m_matchingRepository.Save(entried).ToAsync()
                select savedMatching;
    }
}
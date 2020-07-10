using System;
using LanguageExt;
using Pommel.Server.Domain;
using Pommel.Server.Domain.InGame;
using static LanguageExt.Prelude;

namespace Pommel.Server.UseCase.InGame
{
    public interface ICreateMatchingUseCase
    {
        EitherAsync<IError, IMatching> Execute(string playerId, string playerName);
    }

    public sealed class CreateMatchingUseCase : ICreateMatchingUseCase
    {
        private readonly IPlayerFactory m_playerFactory;

        private readonly IMatchingFactory m_matchingFactory;

        private readonly IMatchingRepository m_matchingRepository;

        public CreateMatchingUseCase(
            IPlayerFactory playerFactory,
            IMatchingFactory matchingFactory,
            IMatchingRepository matchingRepository
            )
        {
            m_playerFactory = playerFactory;
            m_matchingFactory = matchingFactory;
            m_matchingRepository = matchingRepository;
        }

        public EitherAsync<IError, IMatching> Execute(string playerId, string playerName) =>
                from matchingId in RightAsync<IError, string>(Guid.NewGuid().ToString()) // todo ID Generator 的なものをかませる
                from firstPlayer in Try(() => m_playerFactory.Create(playerId, playerName))
                    .ToEitherAsync()
                    .MapLeft(e => new DomainError(e) as IError)
                from matching in Try(() => m_matchingFactory.Create(matchingId, firstPlayer))
                    .ToEitherAsync()
                    .MapLeft(e => new DomainError(e) as IError)
                from savedMatching in m_matchingRepository.Save(matching).ToAsync()
                select savedMatching;

    }
}
using LanguageExt;
using Pommel.Server.Domain;
using Pommel.Server.Domain.InGame;
using static LanguageExt.Prelude;

namespace Pommel.Server.UseCase.InGame
{
    public interface IEnterRoomUseCase
    {
        EitherAsync<IError, IGame> Execute(string gameId, string playerId, string playerName);
    }

    public sealed class EnterRoomUseCase : IEnterRoomUseCase
    {
        private readonly IGameRepository m_gameRepository;

        private readonly IPlayerFactory m_playerFactory;

        public EnterRoomUseCase(
            IGameRepository gameRepository,
            IPlayerFactory playerFactory
            )
        {
            m_gameRepository = gameRepository;
            m_playerFactory = playerFactory;
        }

        public EitherAsync<IError, IGame> Execute(string gameId, string playerId, string playerName) =>
                from game in m_gameRepository.FindById(gameId).ToAsync()
                from player in Try(() => m_playerFactory.Create(playerId, playerName))
                    .ToEitherAsync()
                    .MapLeft(e => new DomainError(e) as IError)
                from entered in Try(() => game.Enter(player))
                    .ToEitherAsync()
                    .MapLeft(e => new DomainError(e) as IError)
                from savedGame in m_gameRepository.Save(entered).ToAsync()
                select savedGame;
    }
}
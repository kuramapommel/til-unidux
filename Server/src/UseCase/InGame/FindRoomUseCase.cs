using LanguageExt;
using Pommel.Server.Domain;
using Pommel.Server.Domain.InGame;

namespace Pommel.Server.UseCase.InGame
{
    public interface IFindGameUseCase
    {
        EitherAsync<IError, IGame> Execute(string gameId);
    }

    public sealed class FindRoomUseCase : IFindGameUseCase
    {
        private readonly IGameRepository m_gameRepository;

        public FindRoomUseCase(
            IGameRepository gameRepository
            )
        {
            m_gameRepository = gameRepository;
        }

        public EitherAsync<IError, IGame> Execute(string gameId) =>
            m_gameRepository.FindById(gameId).ToAsync();
    }
}
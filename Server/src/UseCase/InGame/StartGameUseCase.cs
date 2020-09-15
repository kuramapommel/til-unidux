using System.Threading.Tasks;
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

        private readonly IRoomRepository m_roomRepository;

        public StartGameUseCase(
            IGameRepository gameRepository,
            IRoomRepository roomRepository
            )
        {
            m_gameRepository = gameRepository;
            m_roomRepository = roomRepository;
        }

        public EitherAsync<IError, IGame> Execute(string gameId) =>
                from game in m_gameRepository.FindById(gameId).ToAsync()
                from room in m_roomRepository.FindById(game.MatchingId).ToAsync()
                from matchMakedRoom in Try(() => room.Make())
                    .ToEitherAsync()
                    .MapLeft(e => new DomainError(e) as IError)
                from startedGame in Try(() => game.Start(matchMakedRoom.FirstPlayer.Id))
                    .ToEitherAsync()
                    .MapLeft(e => new DomainError(e) as IError)
                from savedGame in m_roomRepository.Save(matchMakedRoom)
                    .ContinueWith(_ => m_gameRepository.Save(startedGame))
                    .Unwrap()
                    .ToAsync()
                select savedGame;

    }
}
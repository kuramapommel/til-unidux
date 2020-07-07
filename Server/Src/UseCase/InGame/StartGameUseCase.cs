using System;
using System.Linq;
using LanguageExt;
using Pommel.Server.Domain;
using Pommel.Server.Domain.InGame;
using static LanguageExt.Prelude;
using _State = Pommel.Server.Domain.InGame.State;

namespace Pommel.Server.UseCase.InGame
{
    public interface IStartGameUseCase
    {
        EitherAsync<IError, IGame> Execute(string gameId);
    }

    public sealed class StartGameUseCase : IStartGameUseCase
    {
        private readonly IGameRepository m_gameRepository;

        public StartGameUseCase(IGameRepository gameRepository) => m_gameRepository = gameRepository;

        public EitherAsync<IError, IGame> Execute(string gameId) =>
                from game in m_gameRepository.FindById(gameId).ToAsync()
                from started in Try(() => game.Start())
                    .ToEitherAsync()
                    .MapLeft(e => new DomainError(e) as IError)
                from saved in m_gameRepository.Save(started).ToAsync()
                select saved;
    }
}
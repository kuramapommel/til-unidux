using System;
using System.Linq;
using LanguageExt;
using Pommel.Reversi.Domain;
using Pommel.Reversi.Domain.InGame;
using static LanguageExt.Prelude;
using _State = Pommel.Reversi.Domain.InGame.State;

namespace Pommel.Reversi.UseCase.InGame
{
    public interface IStartGameUseCase
    {
        EitherAsync<IError, IGame> Execute();
    }

    public sealed class StartGameUseCase : IStartGameUseCase
    {
        private readonly IGameRepository m_gameRepository;

        public StartGameUseCase(IGameRepository gameRepository) => m_gameRepository = gameRepository;

        public EitherAsync<IError, IGame> Execute() =>
                from games in m_gameRepository.Fetch(game => game.State == _State.NotYet).ToAsync()
                from game in Try(() => games.First())
                    .ToEitherAsync()
                    .MapLeft(_ => new DomainError(new ArgumentOutOfRangeException(), "該当のゲームが存在しません") as IError)
                from started in Try(() => game.Start())
                    .ToEitherAsync()
                    .MapLeft(e => new DomainError(e) as IError)
                from saved in m_gameRepository.Save(started).ToAsync()
                select saved;
    }
}
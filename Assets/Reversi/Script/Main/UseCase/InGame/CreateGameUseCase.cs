using System;
using LanguageExt;
using Pommel.Reversi.Domain;
using Pommel.Reversi.Domain.InGame;
using static LanguageExt.Prelude;

namespace Pommel.Reversi.UseCase.InGame
{
    public interface ICreateGameUseCase
    {
        EitherAsync<IError, IGame> Execute();
    }

    public sealed class CreateGameUseCase : ICreateGameUseCase
    {
        private readonly IGameRepository m_gameRepository;

        private readonly IGameFactory m_gameFactory;

        public CreateGameUseCase(IGameRepository gameRepository, IGameFactory gameFactory)
        {
            m_gameRepository = gameRepository;
            m_gameFactory = gameFactory;
        }

        public EitherAsync<IError, IGame> Execute() =>
                from gameId in RightAsync<IError, string>(Guid.NewGuid().ToString()) // todo ID Generator 的なものをかませる
                from resultId in RightAsync<IError, string>(Guid.NewGuid().ToString()) // todo ID Generator 的なものをかませる
                from saved in m_gameRepository.Save(m_gameFactory.Create(gameId, resultId)).ToAsync()
                select saved;
    }
}
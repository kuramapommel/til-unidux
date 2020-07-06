using System;
using LanguageExt;
using Pommel.Reversi.Domain;
using Pommel.Reversi.Domain.InGame;
using static LanguageExt.Prelude;

namespace Pommel.Reversi.UseCase.InGame
{
    public interface ICreateGameUseCase
    {
        EitherAsync<IError, IGame> Execute(IMatching matching);
    }

    public sealed class CreateGameUseCase : ICreateGameUseCase
    {
        private readonly IGameRepository m_gameRepository;

        private readonly IGameFactory m_gameFactory;

        private readonly ILaidPieceMessagePublisher m_laidPieceMessagePublisher;

        public CreateGameUseCase(
            IGameRepository gameRepository,
            IGameFactory gameFactory,
            ILaidPieceMessagePublisher laidPieceMessagePublisher
            )
        {
            m_gameRepository = gameRepository;
            m_gameFactory = gameFactory;
            m_laidPieceMessagePublisher = laidPieceMessagePublisher;
        }

        public EitherAsync<IError, IGame> Execute(IMatching matching) =>
                from gameId in RightAsync<IError, string>(Guid.NewGuid().ToString()) // todo ID Generator 的なものをかませる
                from resultId in RightAsync<IError, string>(Guid.NewGuid().ToString()) // todo ID Generator 的なものをかませる
                from game in Try(() => m_gameFactory.Create(gameId, resultId, matching.Id))
                    .ToEitherAsync()
                    .MapLeft(e => new DomainError(e) as IError)
                from saved in m_gameRepository.Save(game).ToAsync()
                    .Map(savedGame =>
                    {
                        m_laidPieceMessagePublisher.Join();
                        return savedGame;
                    })
                select saved;
    }
}
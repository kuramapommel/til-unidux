using System;
using LanguageExt;
using Pommel.Server.Domain;
using Pommel.Server.Domain.InGame;
using static LanguageExt.Prelude;

namespace Pommel.Server.UseCase.InGame
{
    public interface ILayPieceUseCase
    {
        EitherAsync<IError, IGame> Execute(string gameId, int x, int y);
    }

    public sealed class LayPieceUseCase : ILayPieceUseCase
    {
        private readonly IGameRepository m_gameRepository;

        private readonly ILaidPieceMessagePublisher m_laidPieceMessagePublisher;

        public LayPieceUseCase(
            IGameRepository gameRepository,
            ILaidPieceMessagePublisher laidPieceMessagePublisher
            )
        {
            m_gameRepository = gameRepository;
            m_laidPieceMessagePublisher = laidPieceMessagePublisher;
        }

        public EitherAsync<IError, IGame> Execute(string gameId, int x, int y) =>
                from point in RightAsync<IError, Point>(new Point(x, y))
                from game in m_gameRepository.FindById(gameId).ToAsync()
                from _ in game.IsValid(game.Turn, point, game.Pieces)
                    ? RightAsync<IError, bool>(true)
                    .Map(validGame =>
                    {
                        m_laidPieceMessagePublisher.Lay(gameId, x, y);
                        return validGame;
                    })
                    : LeftAsync<IError, bool>(new DomainError(new ArgumentOutOfRangeException(), "その場所には置くことができません")) // todo invalid 時の妥当なエラー処理
                from laid in Try(() => game.LayPiece(point))
                    .ToEitherAsync()
                    .MapLeft(e => new DomainError(e) as IError)
                from saved in m_gameRepository.Save(laid).ToAsync()
                select saved;
    }
}
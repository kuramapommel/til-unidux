using System;
using LanguageExt;
using Pommel.Reversi.Domain;
using Pommel.Reversi.Domain.InGame;
using static LanguageExt.Prelude;

namespace Pommel.Reversi.UseCase.InGame
{
    public interface ILayPieceUseCase
    {
        EitherAsync<IError, IGame> Execute(string gameId, int x, int y);
    }

    public sealed class LayPieceUseCase : ILayPieceUseCase
    {
        private readonly IGameRepository m_gameRepository;

        public LayPieceUseCase(IGameRepository gameRepository)
        {
            m_gameRepository = gameRepository;
        }

        public EitherAsync<IError, IGame> Execute(string gameId, int x, int y) =>
                from point in RightAsync<IError, Point>(new Point(x, y))
                from game in m_gameRepository.FindById(gameId).ToAsync()
                from _ in game.IsValide(game.TurnPlayer, point, game.Pieces)
                    ? RightAsync<IError, bool>(true)
                    : LeftAsync<IError, bool>(new DomainError(new ArgumentOutOfRangeException(), "その場所には置くことができません")) // todo invalid 時の妥当なエラー処理
                from laid in RightAsync<IError, IGame>(game.LayPiece(point))
                from saved in m_gameRepository.Save(laid).ToAsync()
                select saved;
    }
}
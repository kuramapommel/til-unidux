using Pommel.Reversi.Domain;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.UseCase.System;
using UniRx.Async;
using System;
using static LanguageExt.Prelude;

namespace Pommel.Reversi.UseCase.InGame
{
    public interface ILayPieceUseCase
    {
        UniTask<IGame> Execute(string gameId, int x, int y);
    }

    public sealed class LayPieceUseCase : ILayPieceUseCase
    {
        private readonly IGameRepository m_gameRepository;

        private readonly IEventPublisher m_publisher;

        public LayPieceUseCase(IGameRepository gameRepository, IEventPublisher publisher)
        {
            m_gameRepository = gameRepository;
            m_publisher = publisher;
        }

        public async UniTask<IGame> Execute(string gameId, int x, int y) =>
            await match(
                from point in RightAsync<IError, Point>(new Point(x, y))
                from game in m_gameRepository.FindById(gameId)
                from _ in game.IsValide(game.TurnPlayer, point, game.Pieces)
                    ? RightAsync<IError, bool>(true)
                    : LeftAsync<IError, bool>(new DomainError(new ArgumentOutOfRangeException(), "その場所には置くことができません")) // todo invalid 時の妥当なエラー処理
                from laid in RightAsync<IError, IGame>(game.LayPiece(point))
                from saved in m_gameRepository.Save(laid)
                select saved,
                Right: game =>
                {
                    _ = m_publisher.Publish<ILaidPieceEvent>(new LaidPieceEvent(game));
                    return game;
                },
                Left: error => throw error.Exception);
    }
}
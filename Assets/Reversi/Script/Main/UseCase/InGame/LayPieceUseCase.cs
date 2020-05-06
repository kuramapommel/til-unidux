using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.UseCase.System;
using UniRx.Async;

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

        public async UniTask<IGame> Execute(string gameId, int x, int y)
        {
            var point = new Point(x, y);
            var game = await m_gameRepository.FindById(gameId);

            // 置けない場合は何もしない
            if (!game.IsValide(game.TurnPlayer, point, game.Pieces)) return game;
            var laid = game.LayPiece(point);
            var savedGame = await m_gameRepository.Save(laid);
            _ = m_publisher.Publish<ILaidPieceEvent>(new LaidPieceEvent(savedGame));
            return savedGame;
        }
    }
}
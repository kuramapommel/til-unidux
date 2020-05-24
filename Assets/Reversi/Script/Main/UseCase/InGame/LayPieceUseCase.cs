using System;
using Pommel.Reversi.Domain;
using Pommel.Reversi.Domain.InGame;
using UniRx;
using UniRx.Async;
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

        private readonly IMessagePublisher m_messagePublisher;

        public LayPieceUseCase(IGameRepository gameRepository, IMessageBroker messagePublisher)
        {
            m_gameRepository = gameRepository;
            m_messagePublisher = messagePublisher;
        }

        public async UniTask<IGame> Execute(string gameId, int x, int y) =>
            await match(
                from point in RightAsync<IError, Point>(new Point(x, y))
                from game in m_gameRepository.FindById(gameId).ToAsync()
                from _ in game.IsValide(game.TurnPlayer, point, game.Pieces)
                    ? RightAsync<IError, bool>(true)
                    : LeftAsync<IError, bool>(new DomainError(new ArgumentOutOfRangeException(), "その場所には置くことができません")) // todo invalid 時の妥当なエラー処理
                from laid in RightAsync<IError, IGame>(game.LayPiece(point))
                from saved in m_gameRepository.Save(laid).ToAsync()
                select saved,
                Right: game =>
                {
                    m_messagePublisher.Publish<ILaidPieceEvent>(new LaidPieceEvent(game));
                    return game;
                },
                Left: error =>
                {
                    UnityEngine.Debug.Log(error.Exception);
                    UnityEngine.Debug.Log(error.Message);
                    throw error.Exception;
                });
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Presentation.Model.InGame;
using UniRx;
using static Pommel.Reversi.Domain.InGame.Winner;

namespace Pommel.Reversi.Presentation.State.InGame
{
    public interface IGameState
    {
        IEnumerable<IPieceState> PieceStates { get; }

        IObservable<IGame> OnStart { get; }

        IObservable<Winner> Winner { get; }

        Task<IGame> CreateGameAsync();

        Task Start();

        Task Finish(Winner winner);

        Task Refresh(IEnumerable<Piece> pieces);
    }

    public sealed class GameState : IGameState
    {
        private readonly IGameModel m_gameModel;

        private readonly IPieceModel m_pieceModel;

        private readonly IPieceStateFactory m_pieceStateFactory;

        private readonly IList<IPieceState> m_pieceStates = new List<IPieceState>();

        private readonly ISubject<IGame> m_onStart = new Subject<IGame>();

        private readonly IReactiveProperty<Winner> m_winner = new ReactiveProperty<Winner>(Undecided);

        public IEnumerable<IPieceState> PieceStates => m_pieceStates;

        public IObservable<IGame> OnStart => m_onStart;

        public IObservable<Winner> Winner => m_winner;

        public GameState(
            IGameModel gameModel,
            IPieceModel pieceModel,
            IPieceStateFactory pieceStateFactory
            )
        {
            m_gameModel = gameModel;
            m_pieceModel = pieceModel;
            m_pieceStateFactory = pieceStateFactory;
        }

        public async Task<IGame> CreateGameAsync() => await m_gameModel.CreateGameAsync();

        public async Task Start()
        {
            var game = await m_gameModel.StartGameAsync();
            foreach (var pieceState in game.Pieces.Select(piece => m_pieceStateFactory.Create(
                game.Id,
                piece.Point,
                piece.Color,
                m_pieceModel)))
            {
                m_pieceStates.Add(pieceState);
            }

            m_gameModel.OnResult
                .Subscribe(result => Finish(result.Winner).AsUniTask().Forget()); // todo add IDisposable

            m_gameModel.OnLaid
                .Subscribe(message => Refresh(message.Game.Pieces).AsUniTask().Forget()); // todo add IDisposable

            m_onStart.OnNext(game);
            m_onStart.OnCompleted();
        }

        public async Task Finish(Winner winner)
        {
            await UniTask.CompletedTask;
            m_winner.Value = winner;
        }

        public async Task Refresh(IEnumerable<Piece> pieces)
        {
            await UniTask.CompletedTask;
            foreach (var (piece, state) in pieces
                .Join(
                    m_pieceStates,
                    pieceEntity => pieceEntity.Point,
                    state => state.Point,
                    (pieceEntity, state) => (pieceEntity, state)
                ))
            {
                state.SetColor(piece.Color.Convert());
            }
        }
    }
}
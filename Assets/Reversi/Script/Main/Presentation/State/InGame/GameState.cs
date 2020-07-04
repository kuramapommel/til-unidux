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

        IPlayerState FirstPlayerState { get; }

        IPlayerState SecondPlayerState { get; }

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

        private readonly IPlayerStateFactory m_playerStateFactory;

        private readonly IList<IPieceState> m_pieceStates = new List<IPieceState>();

        private readonly IDictionary<bool, IPlayerState> m_playerStateMap = new Dictionary<bool, IPlayerState>();

        private readonly ISubject<IGame> m_onStart = new Subject<IGame>();

        private readonly IReactiveProperty<Winner> m_winner = new ReactiveProperty<Winner>(Undecided);

        public IEnumerable<IPieceState> PieceStates => m_pieceStates;

        public IPlayerState FirstPlayerState => m_playerStateMap.TryGetValue(true, out var player)
            ? player
            : throw new InvalidOperationException("先手プレイヤーが初期化されていません");

        public IPlayerState SecondPlayerState => m_playerStateMap.TryGetValue(false, out var player)
            ? player
            : throw new InvalidOperationException("先手プレイヤーが初期化されていません");

        public IObservable<IGame> OnStart => m_onStart;

        public IObservable<Winner> Winner => m_winner;

        public GameState(
            IGameModel gameModel,
            IPieceModel pieceModel,
            IPieceStateFactory pieceStateFactory,
            IPlayerStateFactory playerStateFactory
            )
        {
            m_gameModel = gameModel;
            m_pieceModel = pieceModel;
            m_pieceStateFactory = pieceStateFactory;
            m_playerStateFactory = playerStateFactory;
        }

        public async Task<IGame> CreateGameAsync()
        {
            // todo 一時的に仮実装
            var firstPlayer = (id: Guid.NewGuid().ToString(), name: "左側の人");
            var secondPlayer = (id: Guid.NewGuid().ToString(), name: "migigawa no hito");
            var matching = await m_gameModel.CreateMatchingAsync(firstPlayer.id, firstPlayer.name);
            var entried = await m_gameModel.EntryMatchingAsync(matching.Id, secondPlayer.id, secondPlayer.name);
            var game = await m_gameModel.CreateGameAsync(entried);

            m_playerStateMap.Add(
                true,
                m_playerStateFactory.Create(firstPlayer.id, firstPlayer.name, true));
            m_playerStateMap.Add(
                false,
                m_playerStateFactory.Create(secondPlayer.id, secondPlayer.name, false));

            return game;
        }

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
                .Subscribe(message =>
                {
                    Refresh(message.Pieces).AsUniTask().Forget();

                    var (nextPlayer, notNextPlayer) = message.NextTurnPlayer.Id == FirstPlayerState.Id
                        ? (FirstPlayerState, SecondPlayerState)
                        : (SecondPlayerState, FirstPlayerState);

                    nextPlayer.SetIsNextTurn(true);
                    notNextPlayer.SetIsNextTurn(false);
                }); // todo add IDisposable

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
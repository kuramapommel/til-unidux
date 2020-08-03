using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Presentation.Model.InGame;
using Pommel.Reversi.Presentation.ViewModel.System;
using UniRx;
using _Scene = Pommel.Reversi.Domain.Transition.Scene;

namespace Pommel.Reversi.Presentation.ViewModel.InGame
{
    public interface IGameViewModel : IDisposable
    {
        Task CreateMatchingAsync(string playerId, string playerName);

        Task EntryMatchingAsync(string matchingId, string playerId, string playerName);

        IObservable<IPlayerViewModel> OnInitializeFirstPlayer();

        IObservable<IPlayerViewModel> OnInitializeSecondPlayer();

        IEnumerable<IPieceViewModel> PieceStates { get; }

        IPlayerViewModel FirstPlayerState { get; }

        IPlayerViewModel SecondPlayerState { get; }

        IObservable<IGame> OnStart { get; }

        IObservable<Winner> Winner { get; }
    }

    public sealed class GameViewModel : IGameViewModel
    {
        private readonly IGameModel m_gameModel;

        private readonly IPieceModel m_pieceModel;

        private readonly IPieceStateFactory m_pieceStateFactory;

        private readonly IPlayerStateFactory m_playerStateFactory;

        private readonly IList<IPieceViewModel> m_pieceStates = new List<IPieceViewModel>();

        private readonly IDictionary<bool, IPlayerViewModel> m_playerStateMap = new Dictionary<bool, IPlayerViewModel>();

        private readonly IReactiveProperty<IGame> m_onStart = new ReactiveProperty<IGame>();

        private readonly IReactiveProperty<Winner> m_winner = new ReactiveProperty<Winner>();

        private readonly ISubject<IPlayerViewModel> m_onInitializeFirstPlayer = new Subject<IPlayerViewModel>();

        private readonly ISubject<IPlayerViewModel> m_onInitializeSecondPlayer = new Subject<IPlayerViewModel>();

        public IEnumerable<IPieceViewModel> PieceStates => m_pieceStates;

        public IPlayerViewModel FirstPlayerState => m_playerStateMap.TryGetValue(true, out var player)
            ? player
            : throw new InvalidOperationException("1P が初期化されていません");

        public IPlayerViewModel SecondPlayerState => m_playerStateMap.TryGetValue(false, out var player)
            ? player
            : throw new InvalidOperationException("2P が初期化されていません");

        public IObservable<IGame> OnStart => m_onStart;

        public IObservable<Winner> Winner => m_winner;

        public GameViewModel(
            IGameModel gameModel,
            IPieceModel pieceModel,
            ITransitionState transitionState,
            IPieceStateFactory pieceStateFactory,
            IPlayerStateFactory playerStateFactory
            )
        {
            m_gameModel = gameModel;
            m_pieceModel = pieceModel;
            m_pieceStateFactory = pieceStateFactory;
            m_playerStateFactory = playerStateFactory;

            void refresh(IGame game)
            {
                foreach (var (piece, state) in game.Pieces.Join(
                    m_pieceStates,
                    pieceEntity => pieceEntity.Point,
                    pieceState => pieceState.Point,
                    (pieceEntity, pieceState) => (pieceEntity, pieceState)
                    ))
                {
                    state.SetColor(piece.Color.Convert());
                }

                var (nextPlayer, notNextPlayer) = game.NextTurnPlayerId == FirstPlayerState.Id
                    ? (FirstPlayerState, SecondPlayerState)
                    : (SecondPlayerState, FirstPlayerState);

                nextPlayer.SetIsNextTurn(true);
                notNextPlayer.SetIsNextTurn(false);
            }

            // todo dispose
            m_gameModel.OnJoinAsObservable()
                .Where(matching => matching.SecondPlayer != Player.None)
                .Subscribe(matching =>
                {
                    var firstPlayerState = m_playerStateFactory.Create(matching.FirstPlayer.Id, matching.FirstPlayer.Name, true);
                    m_playerStateMap.Add(
                        true,
                        firstPlayerState
                        );

                    var secondPlayerState = m_playerStateFactory.Create(matching.SecondPlayer.Id, matching.SecondPlayer.Name, false);
                    m_playerStateMap.Add(
                        false,
                        secondPlayerState
                        );

                    _ = transitionState.AddAsync(_Scene.InGame, container => container.Bind<IGameViewModel>().FromInstance(this).AsCached()).AsUniTask()
                        .ContinueWith(() => transitionState.RemoveAsync(_Scene.Title).AsUniTask());
                },
                UnityEngine.Debug.Log);

            // todo dispose
            m_gameModel.OnStartGameAsObservable()
                .Subscribe(game =>
                {
                    foreach (var state in Enumerable.Range(0, 8)
                        .SelectMany(x => Enumerable.Range(0, 8)
                            .Select(y => m_pieceStateFactory.Create(game.Id, new Point(x, y), Color.None, m_pieceModel))))
                    {
                        m_pieceStates.Add(state);
                    }

                    m_onInitializeFirstPlayer.OnNext(FirstPlayerState);
                    m_onInitializeSecondPlayer.OnNext(SecondPlayerState);
                    m_onInitializeFirstPlayer.OnCompleted();
                    m_onInitializeSecondPlayer.OnCompleted();

                    refresh(game);
                    m_onStart.Value = game;
                },
                UnityEngine.Debug.Log);

            // todo dispose
            m_gameModel.OnResultAsObservable()
                .Subscribe(gameResult => m_winner.Value = gameResult.Winner,
                UnityEngine.Debug.Log);

            // todo dispose
            m_gameModel.OnLaidAsObservable()
                .Subscribe(refresh,
                UnityEngine.Debug.Log);
        }

        public async Task CreateMatchingAsync(string playerId, string playerName) => await m_gameModel.CreateMatchingAsync(playerId, playerName);

        public async Task EntryMatchingAsync(string matchingId, string playerId, string playerName) => await m_gameModel.EntryMatchingAsync(matchingId, playerId, playerName);

        public IObservable<IPlayerViewModel> OnInitializeFirstPlayer() => m_onInitializeFirstPlayer;

        public IObservable<IPlayerViewModel> OnInitializeSecondPlayer() => m_onInitializeSecondPlayer;

        void IDisposable.Dispose()
        {
            m_onInitializeFirstPlayer.OnCompleted();
            m_onInitializeSecondPlayer.OnCompleted();
        }
    }
}
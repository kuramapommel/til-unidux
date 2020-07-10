using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Presentation.Model.InGame;
using UniRx;

namespace Pommel.Reversi.Presentation.ViewModel.InGame
{
    public interface IGameViewModel
    {
        Task CreateMatchingAsync(string playerId, string playerName);

        Task EntryMatchingAsync(string matchingId, string playerId, string playerName);

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

        public IEnumerable<IPieceViewModel> PieceStates => m_pieceStates;

        public IPlayerViewModel FirstPlayerState => m_playerStateMap.TryGetValue(true, out var player)
            ? player
            : throw new InvalidOperationException("先手プレイヤーが初期化されていません");

        public IPlayerViewModel SecondPlayerState => m_playerStateMap.TryGetValue(false, out var player)
            ? player
            : throw new InvalidOperationException("先手プレイヤーが初期化されていません");

        public IObservable<IGame> OnStart => m_onStart;

        public IObservable<Winner> Winner => m_winner;

        public GameViewModel(
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

            m_gameModel.OnCreateMatchingAsObservable()
                .Subscribe(matching => m_playerStateMap.Add(
                    true,
                    m_playerStateFactory.Create(matching.FirstPlayer.Id, matching.FirstPlayer.Name, true)));

            m_gameModel.OnJoinAsObservable()
                .Subscribe(matching => m_playerStateMap.Add(
                    false,
                    m_playerStateFactory.Create(matching.SecondPlayer.Id, matching.SecondPlayer.Name, false)));

            m_gameModel.OnStartGameAsObservable()
                .Subscribe(game =>
                {
                    foreach (var state in Enumerable.Range(0, 8)
                        .SelectMany(x => Enumerable.Range(0, 8)
                            .Select(y => m_pieceStateFactory.Create(game.Id, new Point(x, y), Color.None, m_pieceModel))))
                    {
                        m_pieceStates.Add(state);
                    }

                    refresh(game);
                    m_onStart.Value = game;
                });

            m_gameModel.OnResultAsObservable()
                .Subscribe(gameResult => m_winner.Value = gameResult.Winner);

            m_gameModel.OnLaidAsObservable()
                .Subscribe(refresh);
        }

        public async Task CreateMatchingAsync(string playerId, string playerName) => await m_gameModel.CreateMatchingAsync(playerId, playerName);

        public async Task EntryMatchingAsync(string matchingId, string playerId, string playerName) => await m_gameModel.EntryMatchingAsync(matchingId, playerId, playerName);
    }
}
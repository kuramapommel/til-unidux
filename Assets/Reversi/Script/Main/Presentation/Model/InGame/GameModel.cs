using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Infrastructure.Networking.Client;
using UniRx;
using _Color = Pommel.Reversi.Domain.InGame.Color;

namespace Pommel.Reversi.Presentation.Model.InGame
{
    public interface IGameModel : IDisposable
    {
        Task CreateMatchingAsync(string playerId, string playerName);

        Task EntryMatchingAsync(string matchingId, string playerId, string playerName);

        IObservable<IMatching> OnJoinAsObservable();

        IObservable<IGame> OnStartGameAsObservable();

        IObservable<GameResult> OnResultAsObservable();

        IObservable<IGame> OnLaidAsObservable();
    }

    public sealed class GameModel : IGameModel
    {
        private readonly IInGameClient m_client;

        private readonly IPlayerFactory m_playerFactory;

        private readonly IMatchingFactory m_matchingFactory;

        private readonly IGameFactory m_gameFactory;

        private readonly ISubject<IMatching> m_onJoin = new Subject<IMatching>();

        private readonly ISubject<IGame> m_onStartGame = new Subject<IGame>();

        private readonly ISubject<GameResult> m_onResult = new Subject<GameResult>();

        private readonly ISubject<IGame> m_onLay = new Subject<IGame>();

        public GameModel(
            IInGameClient inGameClient,
            IPlayerFactory playerFactory,
            IMatchingFactory matchingFactory,
            IGameFactory gameFactory
            )
        {
            m_client = inGameClient;
            m_playerFactory = playerFactory;
            m_matchingFactory = matchingFactory;
            m_gameFactory = gameFactory;

            // todo dispose
            m_client.OnStartGameAsObservable()
                .Subscribe(arg =>
                {
                    m_onStartGame.OnNext(m_gameFactory.Create(
                        arg.game.Id,
                        arg.nextPlayerId,
                        arg.game.Pieces.Select(piece => new Piece(
                            new Point(piece.X, piece.Y),
                            (_Color)piece.Color
                            ))
                        .ToArray()));
                    m_onStartGame.OnCompleted();
                },
                UnityEngine.Debug.Log);

            // todo dispose
            m_client.OnResultAsObservable()
                .Subscribe(arg =>
                {
                    m_onResult.OnNext(new GameResult(arg.darkCount, arg.lightCount, (Winner)arg.winner));
                    m_onResult.OnCompleted();
                },
                UnityEngine.Debug.Log);

            // todo dispose
            m_client.OnLayAsObservable()
                .Subscribe(arg =>
                {
                    m_onLay.OnNext(m_gameFactory.Create(
                    arg.game.Id,
                    arg.nextPlayerId,
                    arg.game.Pieces.Select(piece => new Piece(
                        new Point(piece.X, piece.Y),
                        (_Color)piece.Color
                        ))
                    .ToArray()));
                },
                UnityEngine.Debug.Log);

            // todo dispose
            m_client.OnJoinAsObservable()
                .Subscribe(arg =>
                {
                    m_onJoin.OnNext(m_matchingFactory.Create(
                        arg.matchingId,
                        m_playerFactory.Create(
                            arg.player1Id,
                            arg.player1Name
                            ),
                        string.IsNullOrEmpty(arg.player2Id)
                        ? Player.None
                        : m_playerFactory.Create(
                            arg.player2Id,
                            arg.player2Name
                            )));
                },
                UnityEngine.Debug.Log
                );
        }

        public async Task CreateMatchingAsync(string playerId, string playerName) =>
            await m_client.CreateMatchingAsync(playerId, playerName).AsUniTask();

        public async Task EntryMatchingAsync(string matchingId, string playerId, string playerName) =>
            await m_client.EntryMatchingAsync(matchingId, playerId, playerName).AsUniTask()
            .ContinueWith(() => m_client.CreateGameAsync(matchingId).AsUniTask());

        public IObservable<IMatching> OnJoinAsObservable() => m_onJoin;

        public IObservable<IGame> OnStartGameAsObservable() => m_onStartGame;

        public IObservable<GameResult> OnResultAsObservable() => m_onResult;

        public IObservable<IGame> OnLaidAsObservable() => m_onLay;

        void IDisposable.Dispose()
        {
            m_onStartGame.OnCompleted();
            m_onResult.OnCompleted();
            m_onLay.OnCompleted();
            m_onJoin.OnCompleted();
        }
    }
}
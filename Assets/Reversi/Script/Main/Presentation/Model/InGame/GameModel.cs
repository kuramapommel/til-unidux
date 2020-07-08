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

        IObservable<IMatching> OnCreateMatchingAsObservable();

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

        public readonly ISubject<IMatching> m_onCreateMatching = new Subject<IMatching>();

        public readonly ISubject<IMatching> m_onJoin = new Subject<IMatching>();

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
            m_client.OnCreateGameAsObservable()
                .Subscribe(arg => m_client.StartAsync(arg.gameId).AsUniTask().ToObservable());

            // todo dispose
            m_client.OnCreateMatchingAsObservable()
                .Subscribe(arg =>
                {
                    _ = m_client.JoinAsync(arg.matchingId, arg.playerId, arg.playerName).AsUniTask();
                    m_onCreateMatching.OnNext(m_matchingFactory.Create(
                        arg.matchingId,
                        m_playerFactory.Create(
                            arg.playerId,
                            arg.playerName
                            )));
                    m_onCreateMatching.OnCompleted();
                });

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
                });

            // todo dispose
            m_client.OnResultAsObservable()
            .Subscribe(arg =>
            {
                m_onResult.OnNext(new GameResult(arg.darkCount, arg.lightCount, (Winner)arg.winner));
                m_onResult.OnCompleted();
            });

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
                });
        }

        public async Task CreateMatchingAsync(string playerId, string playerName)
        {
            // todo dispose
            m_client.OnJoinAsObservable()
                .Subscribe(arg =>
                {
                    m_onJoin.OnNext(m_matchingFactory.Create(
                        arg.matchingId,
                        m_playerFactory.Create(
                            playerId,
                            playerName
                            ),
                        m_playerFactory.Create(
                            arg.playerId,
                            arg.playerName
                            )));
                    m_onJoin.OnCompleted();
                }
                );

            await m_client.CreateMatchingAsync(playerId, playerName).AsUniTask();
        }

        public async Task EntryMatchingAsync(string matchingId, string playerId, string playerName)
        {
            await m_client.JoinAsync(matchingId, playerId, playerName).AsUniTask();
            await m_client.CreateGameAsync(matchingId).AsUniTask();
        }

        public IObservable<IMatching> OnJoinAsObservable() => m_onJoin;

        public IObservable<IMatching> OnCreateMatchingAsObservable() => m_onCreateMatching;

        public IObservable<IGame> OnStartGameAsObservable() => m_onStartGame;

        public IObservable<GameResult> OnResultAsObservable() => m_onResult;

        public IObservable<IGame> OnLaidAsObservable() => m_onLay;

        void IDisposable.Dispose()
        {
            m_onCreateMatching.OnCompleted();
            m_onStartGame.OnCompleted();
            m_onResult.OnCompleted();
            m_onLay.OnCompleted();
            m_onJoin.OnCompleted();
        }
    }
}
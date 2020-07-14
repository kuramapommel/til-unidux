using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MagicOnion.Client;
using Pommel.Api.Hubs;
using Pommel.Api.Services;
using Pommel.Reversi.Domain.InGame;
using UniRx;
using UnityEngine;
using _Channel = Grpc.Core.Channel;
using _ChannelCredentials = Grpc.Core.ChannelCredentials;
using _Color = Pommel.Reversi.Domain.InGame.Color;
using _Game = Pommel.Api.Protocol.InGame.Game;
using _Piece = Pommel.Api.Protocol.InGame.Piece;

namespace Pommel.Reversi.Infrastructure.Networking.Client
{
    public interface IInGameClient : IDisposable, IInGameReceiver
    {
        Task<IGame> SaveAsync(IGame game);

        Task JoinAsync(string matchingId, string playerId, string playerName);

        Task StartAsync(string gameId);

        Task LayAsync(string gameId, int x, int y);

        Task CreateMatchingAsync(string playerId, string playerName);

        Task CreateGameAsync(string matchingId);

        IObservable<(string matchingId, string playerId, string playerName)> OnCreateMatchingAsObservable();

        IObservable<(string matchingId, string playerId, string playerName)> OnJoinAsObservable();

        IObservable<(string gameId, string matchingId)> OnCreateGameAsObservable();

        IObservable<(string nextPlayerId, _Game game)> OnStartGameAsObservable();

        IObservable<(string nextPlayerId, _Game game)> OnLayAsObservable();

        IObservable<(int darkCount, int lightCount, int winner)> OnResultAsObservable();
    }

    public sealed class InGameClient : IInGameClient
    {
        private readonly _Channel m_channel;

        private readonly IInGameService m_inGameService;

        private readonly IInGameHub m_inGameHub;

        private readonly ISubject<(string matchingId, string playerId, string playerName)> m_onCreateMatching = new Subject<(string matchingId, string playerId, string playerName)>();

        private readonly ISubject<(string matchingId, string playerId, string playerName)> m_onJoin = new Subject<(string matchingId, string playerId, string playerName)>();

        private readonly ISubject<(string gameId, string matchingId)> m_onCreateGame = new Subject<(string gameId, string matchingId)>();

        private readonly ISubject<(int darkCount, int whiteCount, int winner)> m_onResult = new Subject<(int darkCount, int whiteCount, int winner)>();

        private readonly ISubject<(string nextPlayerId, _Game game)> m_onStartGame = new Subject<(string nextPlayerId, _Game game)>();

        private readonly ISubject<(string nextPlayerId, _Game game)> m_onLay = new Subject<(string nextPlayerId, _Game game)>();

        public InGameClient()
        {
            m_channel = new _Channel("localhost:12345", _ChannelCredentials.Insecure);
            m_inGameService = MagicOnionClient.Create<IInGameService>(m_channel);
            m_inGameHub = StreamingHubClient.Connect<IInGameHub, IInGameReceiver>(m_channel, this);
        }

        public async Task<IGame> SaveAsync(IGame game)
        {
            int convertColorProtocol(_Color color)
            {
                switch (color)
                {
                    case _Color.None: return 0;
                    case _Color.Dark: return 1;
                    case _Color.Light: return 2;
                }

                return 0;
            }

            var gameprotocol = new _Game
            {
                Id = game.Id,
                Pieces = game.Pieces.Select(piece => new _Piece
                {
                    X = piece.Point.X,
                    Y = piece.Point.Y,
                    Color = convertColorProtocol(piece.Color)
                }).ToArray()
            };

            var response = await m_inGameService.SaveGameAsync(gameprotocol);
            return game;
        }

        public async Task JoinAsync(string matchingId, string playerId, string playerName) => await m_inGameHub.JoinAsync(matchingId, playerId, playerName);

        public async Task StartAsync(string gameId) => await m_inGameHub.StartGameAsync(gameId);

        public async Task LayAsync(string gameId, int x, int y) => await m_inGameHub.LayAsync(gameId, x, y);

        public async Task CreateMatchingAsync(string playerId, string playerName)
        {
            var matchingId = await m_inGameService.CreateMatchingAsync(playerId, playerName);
            Debug.Log($"matchingId is {matchingId}");

            m_onCreateMatching.OnNext((matchingId, playerId, playerName));
            m_onCreateMatching.OnCompleted();
        }

        public async Task CreateGameAsync(string matchingId)
        {
            var gameId = await m_inGameService.CreateGameAsync(matchingId);
            Debug.Log($"gameId is {gameId}");

            m_onCreateGame.OnNext((gameId, matchingId));
            m_onCreateGame.OnCompleted();
        }

        public IObservable<(string matchingId, string playerId, string playerName)> OnCreateMatchingAsObservable() => m_onCreateMatching;

        public IObservable<(string matchingId, string playerId, string playerName)> OnJoinAsObservable() => m_onJoin;

        public IObservable<(string gameId, string matchingId)> OnCreateGameAsObservable() => m_onCreateGame;

        public IObservable<(string nextPlayerId, _Game game)> OnStartGameAsObservable() => m_onStartGame;

        public IObservable<(string nextPlayerId, _Game game)> OnLayAsObservable() => m_onLay;

        public IObservable<(int darkCount, int lightCount, int winner)> OnResultAsObservable() => m_onResult;

        void IInGameReceiver.OnJoin(string matchingId, string playerId, string playerName)
        {
            m_onJoin.OnNext((matchingId, playerId, playerName));
        }

        void IInGameReceiver.OnStartGame(string nextPlayerId, _Game game)
        {
            m_onStartGame.OnNext((nextPlayerId, game));
            m_onStartGame.OnCompleted();
        }

        void IInGameReceiver.OnLay(string nextPlayerId, _Game game)
        {
            m_onLay.OnNext((nextPlayerId, game));
        }

        void IInGameReceiver.OnResult(int darkCount, int lightCount, int winner)
        {
            m_onResult.OnNext((darkCount, lightCount, winner));
            m_onResult.OnCompleted();
        }

        async void IDisposable.Dispose()
        {
            m_onCreateMatching.OnCompleted();
            m_onCreateGame.OnCompleted();
            m_onJoin.OnCompleted();
            m_onLay.OnCompleted();
            m_onStartGame.OnCompleted();
            m_onResult.OnCompleted();
            await m_channel.ShutdownAsync();
            await m_inGameHub.DisposeAsync();
        }
    }
}
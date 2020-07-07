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

        IObservable<string> OnCreateMatchingAsObservable();

        IObservable<(string matchingId, string playerId, string playerName)> OnJoinAsObservable();

        IObservable<(string gameId, string matchingId)> OnCreateGameAsObservable();

        IObservable<_Game> OnStartGameAsObservable();

        IObservable<_Game> OnLayAsObservable();
    }

    public interface IInGameClientFactory
    {
        IInGameClient Create();
    }

    public sealed class InGameClient : IInGameClient
    {
        private readonly _Channel m_channel;

        private readonly IInGameService m_inGameService;

        private readonly IInGameHub m_inGameHub;

        private readonly IReactiveProperty<string> m_onCreateMatching = new ReactiveProperty<string>();

        private readonly IReactiveProperty<(string matchingId, string playerId, string playerName)> m_onJoin = new ReactiveProperty<(string matchingId, string playerId, string playerName)>();

        private readonly IReactiveProperty<(string gameId, string matchingId)> m_onCreateGame = new ReactiveProperty<(string gameId, string matchingId)>();

        private readonly IReactiveProperty<_Game> m_onStartGame = new ReactiveProperty<_Game>();

        private readonly IReactiveProperty<_Game> m_onLay = new ReactiveProperty<_Game>();

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
        }

        public async Task CreateGameAsync(string matchingId)
        {
            var gameId = await m_inGameService.CreateGameAsync(matchingId);
            Debug.Log($"gameId is {gameId}");
        }

        public IObservable<string> OnCreateMatchingAsObservable() => m_onCreateMatching;

        public IObservable<(string matchingId, string playerId, string playerName)> OnJoinAsObservable() => m_onJoin;

        public IObservable<(string gameId, string matchingId)> OnCreateGameAsObservable() => m_onCreateGame;

        public IObservable<_Game> OnStartGameAsObservable() => m_onStartGame;

        public IObservable<_Game> OnLayAsObservable() => m_onLay;

        void IInGameReceiver.OnCreateMatching(string matchingId) => m_onCreateMatching.Value = matchingId;

        void IInGameReceiver.OnJoin(string matchingId, string playerId, string playerName) => m_onJoin.Value = (matchingId, playerId, playerName);

        void IInGameReceiver.OnCreateGame(string gameId, string matchingId) => m_onCreateGame.Value = (gameId, matchingId);

        void IInGameReceiver.OnStartGame(_Game game) => m_onStartGame.Value = game;

        void IInGameReceiver.OnLay(_Game game) => m_onLay.Value = game;

        void IInGameReceiver.OnResult(int darkCount, int lightCount, int winner)
        {
            // todo result イベント発火
        }

        async void IDisposable.Dispose()
        {
            await m_channel.ShutdownAsync();
            await m_inGameHub.DisposeAsync();
        }
    }
}
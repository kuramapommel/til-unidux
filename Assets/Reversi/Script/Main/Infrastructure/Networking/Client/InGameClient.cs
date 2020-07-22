using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MagicOnion.Client;
using Pommel.Api.Hubs;
using Pommel.Api.Services;
using Pommel.Reversi.Domain.InGame;
using UniRx;
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

        Task StartAsync(string gameId);

        Task LayAsync(string gameId, int x, int y);

        Task CreateMatchingAsync(string playerId, string playerName);

        Task EntryMatchingAsync(string matchingId, string playerId, string playerName);

        Task CreateGameAsync(string matchingId);

        IObservable<(string matchingId, string player1Id, string player1Name, string player2Id, string player2Name)> OnJoinAsObservable();

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

        private readonly ISubject<(string matchingId, string player1Id, string player1Name, string player2Id, string player2Name)> m_onJoin = new Subject<(string matchingId, string player1Id, string player1Name, string player2Id, string player2Name)>();

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

        async Task IInGameClient.StartAsync(string gameId) =>
            await m_inGameHub.StartGameAsync(gameId);

        async Task IInGameClient.LayAsync(string gameId, int x, int y) =>
            await m_inGameHub.LayAsync(gameId, x, y);

        async Task IInGameClient.CreateMatchingAsync(string playerId, string playerName) =>
            await m_inGameHub.CreateMatchingAsync(playerId, playerName);

        async Task IInGameClient.EntryMatchingAsync(string matchingId, string playerId, string playerName) =>
            await m_inGameHub.EntryMatchingAsync(matchingId, playerId, playerName);

        async Task IInGameClient.CreateGameAsync(string matchingId) =>
            await m_inGameHub.CreateGameAsync(matchingId);

        IObservable<(string matchingId, string player1Id, string player1Name, string player2Id, string player2Name)> IInGameClient.OnJoinAsObservable() => m_onJoin;

        IObservable<(string gameId, string matchingId)> IInGameClient.OnCreateGameAsObservable() => m_onCreateGame;

        IObservable<(string nextPlayerId, _Game game)> IInGameClient.OnStartGameAsObservable() => m_onStartGame;

        IObservable<(string nextPlayerId, _Game game)> IInGameClient.OnLayAsObservable() => m_onLay;

        IObservable<(int darkCount, int lightCount, int winner)> IInGameClient.OnResultAsObservable() => m_onResult;

        void IInGameReceiver.OnJoin(string matchingId, string player1Id, string player1Name, string player2Id, string player2Name)
        {
            m_onJoin.OnNext((matchingId, player1Id, player1Name, player2Id, player2Name));
            UnityEngine.Debug.Log($"matchingId is {matchingId}, player1Id {player1Id}, player1Name {player1Name}, player2Id {player2Id}, player2Name {player2Name}");
        }

        void IInGameReceiver.OnStartGame(string nextPlayerId, _Game game)
        {
            m_onStartGame.OnNext((nextPlayerId, game));
            m_onStartGame.OnCompleted();
            UnityEngine.Debug.Log($"nextPlayerId is {nextPlayerId}, game id is {game}, piece is {game.Pieces.Aggregate(string.Empty, (aggregate, piece) => $"{aggregate} | x = {piece.X}, y = {piece.Y}, color = {piece.Color}")}");
        }

        void IInGameReceiver.OnLay(string nextPlayerId, _Game game)
        {
            m_onLay.OnNext((nextPlayerId, game));
            UnityEngine.Debug.Log($"nextPlayerId is {nextPlayerId}, game id is {game}, piece is {game.Pieces.Aggregate(string.Empty, (aggregate, piece) => $"{aggregate} | x = {piece.X}, y = {piece.Y}, color = {piece.Color}")}");
        }

        void IInGameReceiver.OnResult(int darkCount, int lightCount, int winner)
        {
            m_onResult.OnNext((darkCount, lightCount, winner));
            m_onResult.OnCompleted();
        }

        void IInGameReceiver.OnCreateGame(string gameId, string matchingId)
        {
            m_onCreateGame.OnNext((gameId, matchingId));
            m_onCreateGame.OnCompleted();
        }

        async void IDisposable.Dispose()
        {
            m_onCreateGame.OnCompleted();
            m_onJoin.OnCompleted();
            m_onLay.OnCompleted();
            m_onStartGame.OnCompleted();
            m_onResult.OnCompleted();

            await m_inGameHub.DisposeAsync();
            await m_channel.ShutdownAsync();
        }
    }
}
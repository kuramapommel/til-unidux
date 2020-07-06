using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MagicOnion.Client;
using Pommel.Api.Hubs;
using Pommel.Api.Services;
using Pommel.Reversi.Domain.InGame;
using _Channel = Grpc.Core.Channel;
using _ChannelCredentials = Grpc.Core.ChannelCredentials;
using _Game = Pommel.Api.Protocol.InGame.Game;
using _Piece = Pommel.Api.Protocol.InGame.Piece;

namespace Pommel.Reversi.Infrastructure.Networking.Client
{
    public interface IInGameClient : IDisposable, IInGameReceiver
    {
        Task<IGame> SaveAsync(IGame game);

        Task JoinAsync();

        Task LayAsync(string gameId, int x, int y);
    }

    public interface IInGameClientFactory
    {
        IInGameClient Create(Action<string, int, int> onLay = default);
    }

    public sealed class InGameClient : IInGameClient
    {
        private readonly _Channel m_channel;

        private readonly IInGameService m_inGameService;

        private readonly IInGameHub m_inGameHub;

        private readonly Action<string, int, int> m_onLay;

        public InGameClient(Action<string, int, int> onLay)
        {
            m_channel = new _Channel("localhost:12345", _ChannelCredentials.Insecure);
            m_inGameService = MagicOnionClient.Create<IInGameService>(m_channel);
            m_inGameHub = StreamingHubClient.Connect<IInGameHub, IInGameReceiver>(m_channel, this);
            m_onLay = onLay;
        }

        public async Task<IGame> SaveAsync(IGame game)
        {
            int convertColorProtocol(Color color)
            {
                switch (color)
                {
                    case Color.None: return 0;
                    case Color.Dark: return 1;
                    case Color.Light: return 2;
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
        public async Task JoinAsync() => await m_inGameHub.JoinAsync();

        public async Task LayAsync(string gameId, int x, int y) => await m_inGameHub.LayAsync(gameId, x, y);

        public void OnLay(string gameId, int x, int y) => m_onLay(gameId, x, y);

        public async void Dispose()
        {
            await m_channel.ShutdownAsync();
        }
    }
}
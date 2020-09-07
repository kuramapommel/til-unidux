using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MagicOnion.Client;
using Pommel.Api.Hubs;
using Pommel.Api.Services;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Reducks.InGame;
using _Channel = Grpc.Core.Channel;
using _ChannelCredentials = Grpc.Core.ChannelCredentials;

namespace Pommel.Reversi.Infrastructure.Client
{
    public sealed class InGameClient : IInGameReceiver, IClient, IDisposable
    {
        private _Channel m_channel;

        private IInGameService m_inGameService;

        private IInGameHub m_inGameHub;

        private readonly IOperation m_operation;

        private readonly IProps m_props;

        public InGameClient(
            IOperation operation,
            IProps props
            )
        {
            m_operation = operation;
            m_props = props;
        }

        void IInGameReceiver.OnRefresh(Api.Protocol.InGame.Game game)
        {
            ValueObjects.Stone.Color convertColor(int color)
            {
                switch (color)
                {
                    case 0: return ValueObjects.Stone.Color.None;
                    case 1: return ValueObjects.Stone.Color.Dark;
                    case 2: return ValueObjects.Stone.Color.Light;
                }

                throw new IndexOutOfRangeException("不正な値");
            }

            ValueObjects.State convertState(int state)
            {
                switch (state)
                {
                    case 0: return ValueObjects.State.NotYet;
                    case 1: return ValueObjects.State.Playing;
                    case 2: return ValueObjects.State.Finished;
                }

                throw new IndexOutOfRangeException("不正な値");
            }

            _ = m_operation.RefreshAndNextTurn(
                new ValueObjects.Game(
                game.Id,
                new ValueObjects.Room(
                    game.Room.Id,
                    new ValueObjects.Room.Player(
                        game.Room.FirstPlayer.Id,
                        game.Room.FirstPlayer.Name,
                        game.Room.FirstPlayer.IsLight ? ValueObjects.Stone.Color.Light : ValueObjects.Stone.Color.Dark,
                        game.Room.FirstPlayer.IsTurnPlayer
                        ),
                    new ValueObjects.Room.Player(
                        game.Room.SecondPlayer.Id,
                        game.Room.SecondPlayer.Name,
                        game.Room.SecondPlayer.IsLight ? ValueObjects.Stone.Color.Light : ValueObjects.Stone.Color.Dark,
                        game.Room.SecondPlayer.IsTurnPlayer
                        )
                    ),
                game.Pieces
                .Select(piece => new ValueObjects.Stone(
                    new ValueObjects.Point(
                        piece.X,
                        piece.Y),
                    convertColor(piece.Color)
                ))
                .ToArray(),
                convertState(game.State)
                ));
        }

        async Task IClient.ConnectAsync()
        {
            // todo domain error
            if (m_channel != null || m_inGameService != null || m_inGameService != null) throw new InvalidOperationException("すでに接続されています");

            await UniTask.CompletedTask;

            m_channel = new _Channel("localhost:12345", _ChannelCredentials.Insecure);
            m_inGameService = MagicOnionClient.Create<IInGameService>(m_channel);
            m_inGameHub = StreamingHubClient.Connect<IInGameHub, IInGameReceiver>(m_channel, this);
        }

        async Task IClient.DisconnectAsync()
        {
            await m_inGameHub.DisposeAsync();
            await m_channel.ShutdownAsync();
        }

        async Task IClient.PutStoneAsync(int x, int y) => await m_inGameHub.LayAsync(x, y);

        async Task IClient.CreateGameAsync(string roomId) => await m_inGameHub.CreateGameAsync(roomId);

        async Task<string> IClient.CreateRoomAsync() => await m_inGameService.CreateRoomAsync();

        async Task<string> IClient.EntryRoomAsync(string roomId, string playerId, string playerName) =>
            await m_inGameService.EntryRoomAsync(roomId, playerId, playerName);

        async Task<ValueObjects.Room> IClient.FindRoomById(string roomId) =>
            await m_inGameService.FindRoomById(roomId)
                .ResponseAsync
                .AsUniTask()
                .ContinueWith(room => new ValueObjects.Room(
                    room.Id,
                    new ValueObjects.Room.Player(
                        room.FirstPlayer.Id,
                        room.FirstPlayer.Name,
                        room.FirstPlayer.IsLight ? ValueObjects.Stone.Color.Light : ValueObjects.Stone.Color.Dark,
                        room.FirstPlayer.IsTurnPlayer
                        ),
                    new ValueObjects.Room.Player(
                        room.SecondPlayer.Id,
                        room.SecondPlayer.Name,
                        room.SecondPlayer.IsLight ? ValueObjects.Stone.Color.Light : ValueObjects.Stone.Color.Dark,
                        room.SecondPlayer.IsTurnPlayer
                        )
                    ));

        void IDisposable.Dispose()
        {
            // ちゃんと IClient.DisconnectAsyncImpl() を呼ばなかった場合（主にアプリクラッシュ時）
            m_inGameHub.DisposeAsync()
                .ContinueWith(_ => m_channel.ShutdownAsync())
                .Unwrap()
                .Wait();
        }
    }
}
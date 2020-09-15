using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MagicOnion.Client;
using Pommel.Api.Hubs;
using Pommel.Api.Services;
using Pommel.Reversi.Domain.InGame;
using static Pommel.Reversi.Reducks.InGame.Operations;
using static Pommel.Reversi.Reducks.Title.Operations;
using _Channel = Grpc.Core.Channel;
using _ChannelCredentials = Grpc.Core.ChannelCredentials;

namespace Pommel.Reversi.Infrastructure.Client
{
    public sealed class InGameClient : IInGameReceiver, IClient, IDisposable
    {
        private readonly _Channel m_channel;

        private readonly IInGameService m_inGameService;

        private IInGameHub m_inGameHub;

        private readonly IRefreshableAndNextTurn m_refreshableAndNextTurn;

        private readonly IStartableGame m_startableGame;

        private readonly IDispatcher m_dispatcher;

        private readonly IProps m_props;

        public InGameClient(
            IRefreshableAndNextTurn refreshableAndNextTurn,
            IStartableGame startableGame,
            IProps props,
            IDispatcher dispatcher
            )
        {
            m_refreshableAndNextTurn = refreshableAndNextTurn;
            m_startableGame = startableGame;
            m_props = props;
            m_dispatcher = dispatcher;

            m_channel = new _Channel("localhost:12345", _ChannelCredentials.Insecure);
            m_inGameService = MagicOnionClient.Create<IInGameService>(m_channel);
        }

        void IInGameReceiver.OnRefresh(Api.Protocol.InGame.Game game) => m_dispatcher.Dispatch(m_refreshableAndNextTurn.RefreshAndNextTurn(game.Convert()));

        void IInGameReceiver.OnStart(Api.Protocol.InGame.Game game) => m_dispatcher.Dispatch(m_startableGame.StartGame(game.Convert()));

        async Task IClient.ConnectAsync()
        {
            await UniTask.CompletedTask;

            m_inGameHub = StreamingHubClient.Connect<IInGameHub, IInGameReceiver>(m_channel, this);
        }

        async Task IClient.DisconnectAsync()
        {
            await m_inGameHub.DisposeAsync();
            await m_channel.ShutdownAsync();
        }

        async Task IClient.PutStoneAsync(int x, int y) => await m_inGameHub.LayAsync(x, y);

        async Task IClient.EntryRoomAsync(string roomId, string playerId, string playerName) =>
            await m_inGameHub.EntryRoomAsync(roomId, playerId, playerName);

        async Task IClient.StartGameAsync(string gameId) => await m_inGameHub.StartGameAsync(gameId);

        async Task<string> IClient.CreateGameAsync(string roomId) => await m_inGameService.CreateGameAsync(roomId);

        async Task<string> IClient.CreateRoomAsync() => await m_inGameService.CreateRoomAsync();

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

    public static class GameExt
    {
        public static ValueObjects.Game Convert(this Api.Protocol.InGame.Game game)
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

            return new ValueObjects.Game(
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
                convertState(game.State));
        }
    }
}
using System.Threading.Tasks;
using MagicOnion.Client;
using Pommel.Api.Hubs;
using Pommel.Api.Services;
using Pommel.Reversi.Reducks.InGame;
using System.Linq;
using _Channel = Grpc.Core.Channel;
using _ChannelCredentials = Grpc.Core.ChannelCredentials;

namespace Pommel.Reversi.Infrastructure.InGame
{
    public sealed class Client : IInGameReceiver
    {
        private readonly _Channel m_channel;

        private readonly IInGameService m_inGameService;

        private readonly IInGameHub m_inGameHub;

        private readonly IOperation m_operation;

        private readonly IProps m_props;

        public Client(
            IOperation operation,
            IProps props,
            _Channel channel = null,
            IInGameService inGameService = null,
            IInGameHub inGameHub = null
            )
        {
            m_operation = operation;
            m_props = props;
            m_channel = channel;
            m_inGameService = inGameService;
            m_inGameHub = inGameHub;
        }

        public async Task<Client> DisconnectAsyncImpl()
        {
            await m_inGameHub.DisposeAsync();
            await m_channel.ShutdownAsync();

            return this;
        }

        public Task<Client> ConnectAsyncImpl()
        {
            // todo domain error
            if (m_channel != null || m_inGameService != null || m_inGameService != null) return Task.FromException<Client>(new System.Exception("すでに接続されています"));

            var channel = new _Channel("localhost:12345", _ChannelCredentials.Insecure);
            var inGameService = MagicOnionClient.Create<IInGameService>(channel);
            var inGameHub = StreamingHubClient.Connect<IInGameHub, IInGameReceiver>(channel, this);

            return Task.FromResult(new Client(
                m_operation,
                m_props,
                channel,
                inGameService,
                inGameHub
                ));
        }

        public async Task PutStoneAsync(int x, int y)
        {
            var gameId = m_props.InGame.Id;
            await m_inGameHub.LayAsync(gameId, x, y);
        }

        /*
         * 此処から先は operation を叩く子たち 
         */
        void IInGameReceiver.OnRefresh(Api.Protocol.InGame.Game game)
        {
            ValueObject.Stone.Color convert(int color)
            {
                switch (color)
                {
                    case 0: return ValueObject.Stone.Color.None;
                    case 1: return ValueObject.Stone.Color.Dark;
                    case 2: return ValueObject.Stone.Color.Light;
                }

                throw new System.IndexOutOfRangeException("値が不正");
            }

            var stones = game.Pieces
                .Select(piece => new ValueObject.Stone(
                    new ValueObject.Point(
                        piece.X,
                        piece.Y),
                    convert(piece.Color)
                ))
                .ToArray();

            var nextPlayerId = game.Room.FirstPlayer.IsTurnPlayer
                ? game.Room.FirstPlayer.Id
                : game.Room.SecondPlayer.Id;

            m_operation.RefreshAndNextTurn(stones, nextPlayerId);
        }


        void IInGameReceiver.OnJoin(string matchingId, string player1Id, string player1Name, string player2Id, string player2Name)
        {
            throw new System.NotImplementedException();
        }

        void IInGameReceiver.OnLay(string nextPlayerId, Api.Protocol.InGame.Game game)
        {
            throw new System.NotImplementedException();
        }

        void IInGameReceiver.OnResult(int darkCount, int lightCount, int winner)
        {
            throw new System.NotImplementedException();
        }

        void IInGameReceiver.OnStartGame(string nextPlayerId, string matchingId, Api.Protocol.InGame.Game game)
        {
            throw new System.NotImplementedException();
        }
    }
}
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MagicOnion;
using MagicOnion.Server;
using MagicOnion.Server.Hubs;
using Pommel.Api.Hubs;
using Pommel.Server.Controller.Filter;
using Pommel.Server.UseCase.InGame;

namespace Pommel.Server.Controller.Hub
{
    public sealed class InGameHub : StreamingHubBase<IInGameHub, IInGameReceiver>, IInGameHub
    {
        private readonly ILayPieceUseCase m_layPieceUseCase;

        private readonly IEnterRoomUseCase m_enterRoomUseCase;

        private readonly IStartGameUseCase m_startGameUseCase;

        private IGroup m_room;

        private string m_playerId = string.Empty;

        private string m_playerName = string.Empty;

        public InGameHub(
            ILayPieceUseCase layPieceUseCase,
            IEnterRoomUseCase enterRoomUseCase,
            IStartGameUseCase startGameUseCase
            )
        {
            m_layPieceUseCase = layPieceUseCase;
            m_enterRoomUseCase = enterRoomUseCase;
            m_startGameUseCase = startGameUseCase;
        }

        [FromTypeFilter(typeof(ExceptionHandlingFilterAttribute))]
        async Task IInGameHub.EntryRoomAsync(string roomId, string playerId, string playerName) =>
            await m_enterRoomUseCase.Execute(roomId, playerId, playerName)
                .Match(
                    Right: async matching =>
                    {
                        m_playerId = playerId;
                        m_playerName = playerName;
                        m_room = await Group.AddAsync(matching.Id);
                    },
                    // todo エラーの内容を見て正しくハンドリング
                    Left: error => throw new ReturnStatusException((Grpc.Core.StatusCode)99, error.Message)
                )
                .Unwrap();

        [FromTypeFilter(typeof(ExceptionHandlingFilterAttribute))]
        async Task IInGameHub.LayAsync(int x, int y) =>
            await m_layPieceUseCase.Execute(m_room.GroupName, x, y)
                .Match(
                    Right: game => Broadcast(m_room)
                        .OnRefresh(
                            new Api.Protocol.InGame.Game()
                            {
                                Id = game.Id,
                                Pieces = game.Pieces
                                .Select(piece => new Api.Protocol.InGame.Piece()
                                {
                                })
                                .ToArray(),
                                Room = new Api.Protocol.InGame.Room()
                                // todo domain service 作るべき
                                //{
                                //    Id = game.MatchingId,
                                //    FirstPlayer = game.
                                //}
                                ,
                                State = 2 // todo 
                            }
                        ),
                    //.OnLay(
                    //    game.NextTurnPlayerId,
                    //    new _Game
                    //    {
                    //        Id = game.Id,
                    //        Pieces = game.Pieces
                    //            .Select(piece => new _Piece
                    //            {
                    //                X = piece.Point.X,
                    //                Y = piece.Point.Y,
                    //                Color = (int)piece.Color
                    //            })
                    //            .ToArray()
                    //    }),
                    // todo エラーの内容を見て正しくハンドリング
                    Left: error => throw new ReturnStatusException((Grpc.Core.StatusCode)99, error.Message)
                );

        [FromTypeFilter(typeof(ExceptionHandlingFilterAttribute))]
        async Task IInGameHub.StartGameAsync(string gameId) =>
            await m_startGameUseCase.Execute(gameId)
                .Match(
                    Right: game => Broadcast(m_room)
                        .OnStart(
                            new Api.Protocol.InGame.Game()
                            {
                                Id = game.Id,
                                Pieces = game.Pieces
                                .Select(piece => new Api.Protocol.InGame.Piece()
                                {
                                })
                                .ToArray(),
                                Room = new Api.Protocol.InGame.Room()
                                // todo domain service 作るべき
                                //{
                                //    Id = game.MatchingId,
                                //    FirstPlayer = game.
                                //}
                                ,
                                State = 2 // todo 
                            }
                        ),
                    //.OnLay(
                    //    game.NextTurnPlayerId,
                    //    new _Game
                    //    {
                    //        Id = game.Id,
                    //        Pieces = game.Pieces
                    //            .Select(piece => new _Piece
                    //            {
                    //                X = piece.Point.X,
                    //                Y = piece.Point.Y,
                    //                Color = (int)piece.Color
                    //            })
                    //            .ToArray()
                    //    }),
                    // todo エラーの内容を見て正しくハンドリング
                    Left: error => throw new ReturnStatusException((Grpc.Core.StatusCode)99, error.Message)
                );
    }
}
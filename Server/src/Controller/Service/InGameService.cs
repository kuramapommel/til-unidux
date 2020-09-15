using MagicOnion;
using MagicOnion.Server;
using Pommel.Api.Protocol.InGame;
using Pommel.Api.Services;
using Pommel.Server.UseCase.InGame;

namespace Pommel.Server.Controller.Service
{
    public sealed class InGameService : ServiceBase<IInGameService>, IInGameService
    {
        private readonly ICreateRoomUseCase m_createRoomUseCase;

        private readonly IEnterRoomUseCase m_enterRoomUseCase;

        private readonly IFindRoomUseCase m_findRoomUseCase;

        public InGameService(
            ICreateRoomUseCase createRoomUseCase,
            IEnterRoomUseCase enterRoomUseCase,
            IFindRoomUseCase findRoomUseCase
            )
        {
            m_createRoomUseCase = createRoomUseCase;
            m_enterRoomUseCase = enterRoomUseCase;
            m_findRoomUseCase = findRoomUseCase;
        }

        async UnaryResult<string> IInGameService.CreateRoomAsync() =>
            await m_createRoomUseCase.Execute()
                .Match(
                    Right: room => room.Id,
                    // todo エラーの内容を見て正しくハンドリング
                    Left: error => throw new ReturnStatusException((Grpc.Core.StatusCode)99, error.Message)
                );

        async UnaryResult<string> IInGameService.EntryRoomAsync(string roomId, string playerId, string playerName) =>
            await m_enterRoomUseCase.Execute(roomId, playerId, playerName)
                .Match(
                    Right: room => room.Id,
                    // todo エラーの内容を見て正しくハンドリング
                    Left: error => throw new ReturnStatusException((Grpc.Core.StatusCode)99, error.Message)
                );

        async UnaryResult<Room> IInGameService.FindRoomById(string roomId) =>
            await m_findRoomUseCase.Execute(roomId)
                .Match(
                    Right: room => new Room()
                    {
                        Id = room.Id,
                        FirstPlayer = new Player()
                        {
                            Id = room.FirstPlayer.Id,
                            Name = room.FirstPlayer.Name,
                            IsLight = true, // todo FirstPlayer にプロパティもたせる
                            IsTurnPlayer = true // todo FirstPlayer にプロパティもたせる
                        },
                        SecondPlayer = new Player()
                        {
                            Id = room.SecondPlayer.Id,
                            Name = room.SecondPlayer.Name,
                            IsLight = false, // todo FirstPlayer にプロパティもたせる
                            IsTurnPlayer = false // todo FirstPlayer にプロパティもたせる
                        }
                    },
                    // todo エラーの内容を見て正しくハンドリング
                    Left: error => throw new ReturnStatusException((Grpc.Core.StatusCode)99, error.Message)
                );
    }
}
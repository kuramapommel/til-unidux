using System.Linq;
using MagicOnion;
using MagicOnion.Server;
using Pommel.Api.Services;
using Pommel.Server.Domain.InGame;
using Pommel.Server.UseCase.InGame;

namespace Pommel.Server.Controller.Service
{
    public sealed class InGameService : ServiceBase<IInGameService>, IInGameService
    {
        private readonly ICreateGameUseCase m_createGameUseCase;

        private readonly IEnterRoomUseCase m_enterRoomUseCase;

        private readonly IFindGameUseCase m_findRoomUseCase;

        public InGameService(
            ICreateGameUseCase createGameUseCase,
            IEnterRoomUseCase enterRoomUseCase,
            IFindGameUseCase findRoomUseCase
            )
        {
            m_createGameUseCase = createGameUseCase;
            m_enterRoomUseCase = enterRoomUseCase;
            m_findRoomUseCase = findRoomUseCase;
        }

        async UnaryResult<string> IInGameService.CreateGameAsync() =>
            await m_createGameUseCase.Execute()
                .Match(
                    Right: game => game.Id,
                    // todo エラーの内容を見て正しくハンドリング
                    Left: error => throw new ReturnStatusException((Grpc.Core.StatusCode)99, error.Message)
                );

        async UnaryResult<string> IInGameService.EntryRoomAsync(string roomId, string playerId, string playerName) =>
            await m_enterRoomUseCase.Execute(roomId, playerId, playerName)
                .Match(
                    Right: game => game.Id,
                    // todo エラーの内容を見て正しくハンドリング
                    Left: error => throw new ReturnStatusException((Grpc.Core.StatusCode)99, error.Message)
                );

        async UnaryResult<Api.Protocol.InGame.Game> IInGameService.FindGameById(string gameId) =>
            await m_findRoomUseCase.Execute(gameId)
                .Match(
                    Right: game => new Api.Protocol.InGame.Game()
                            {
                                Id = game.Id,
                                Pieces = game.Pieces
                                .Select(piece => new Api.Protocol.InGame.Piece()
                                {
                                    X = piece.Point.X,
                                    Y = piece.Point.Y,
                                    Color = piece.Color.ToInt()
                                })
                                .ToArray(),
                                Room = new Api.Protocol.InGame.Room()
                                {
                                    FirstPlayer = new Api.Protocol.InGame.Player()
                                    {
                                        Id = game.Room.FirstPlayer.Id,
                                        Name = game.Room.FirstPlayer.Name,
                                        IsTurnPlayer = game.NextTurnPlayerId == game.Room.FirstPlayer.Id,
                                        IsLight = true
                                    },
                                    SecondPlayer = new Api.Protocol.InGame.Player()
                                    {
                                        Id = game.Room.SecondPlayer.Id,
                                        Name = game.Room.SecondPlayer.Name,
                                        IsTurnPlayer = game.NextTurnPlayerId == game.Room.SecondPlayer.Id,
                                        IsLight = false
                                    }
                                },
                                State = game.State.ToInt()
                            },
                    // todo エラーの内容を見て正しくハンドリング
                    Left: error => throw new ReturnStatusException((Grpc.Core.StatusCode)99, error.Message)
                );
    }
}
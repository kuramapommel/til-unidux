using System.Threading.Tasks;
using MagicOnion;
using MagicOnion.Server;
using Pommel.Api.Protocol.InGame;
using Pommel.Api.Services;
using Pommel.Server.UseCase.InGame;

namespace Pommel.Server.Infrastructure.Controller.Service
{
    public sealed class InGameService : ServiceBase<IInGameService>, IInGameService
    {
        private readonly ICreateMatchingUseCase m_createMatchingUseCase;

        private readonly ICreateGameUseCase m_createGameUseCase;

        public InGameService(
            ICreateMatchingUseCase createMatchingUseCase,
            ICreateGameUseCase createGameUseCase
            )
        {
            m_createMatchingUseCase = createMatchingUseCase;
            m_createGameUseCase = createGameUseCase;
        }

        public async UnaryResult<string> CreateMatchingAsync(string playerId, string playerName) =>
            await m_createMatchingUseCase.Execute(playerId, playerName)
                .Match(
                    Right: matching => matching.Id,
                    // todo エラーの内容を見て正しくハンドリング
                    Left: error => throw new ReturnStatusException((Grpc.Core.StatusCode)99, error.Message)
                );

        public async UnaryResult<string> CreateGameAsync(string matchingId) =>
            await m_createGameUseCase.Execute(matchingId)
                .Match(
                    Right: game => game.Id,
                    // todo エラーの内容を見て正しくハンドリング
                    Left: error => throw new ReturnStatusException((Grpc.Core.StatusCode)99, error.Message)
                );

        public async UnaryResult<Game> SaveGameAsync(Game game)
        {
            Logger.Debug($"game id is {game.Id}");
            foreach (var piece in game.Pieces)
                Logger.Debug($"piece x is {piece.X}, piece y is {piece.Y}");

            await Task.CompletedTask;
            return game;
        }
    }
}
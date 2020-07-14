using System.Threading.Tasks;
using Grpc.Core;
using MagicOnion.Hosting;
using MagicOnion.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pommel.Server.Component.Reactive;
using Pommel.Server.Domain.InGame;
using Pommel.Server.Infrastructure.DomainService.InGame;
using Pommel.Server.Infrastructure.Repository.InGame;
using Pommel.Server.Infrastructure.Service.InGame;
using Pommel.Server.Infrastructure.Store.InGame;
using Pommel.Server.UseCase.InGame;
using Pommel.Server.UseCase.InGame.Message;

namespace Pommel.Server
{
    public static class Program
    {
        // MagicOnion を await で起動するため、非同期 Main で定義
        // await でサーバーを起動しないと、即 Main 関数が終了してしまうため
        public static async Task Main(string[] args)
        {
            // gRPC のログをコンソールに出力するよう設定
            GrpcEnvironment.SetLogger(new Grpc.Core.Logging.ConsoleLogger());

            // isReturnExceptionStackTraceInErrorDetail に true を設定して
            // エラー発生時のメッセージがコンソールに出力されるようにする
            // MagicOnion サーバーが localhost:12345 で Listen する
            await MagicOnionHost.CreateDefaultBuilder()
                .UseMagicOnion(
                    new MagicOnionOptions(isReturnExceptionStackTraceInErrorDetail: true),
                    new ServerPort("0.0.0.0", 12345, ServerCredentials.Insecure))
                .ConfigureServices((hostContext, services) =>
                {
                    // todo 分割しないとやばいことになるので分割する
                    // dependency injection
                    services.AddSingleton(MessageBroker<IResultMessage>.CreateInstance());

                    services.AddSingleton(GameStore.Instance);
                    services.AddSingleton(GameResultStore.Instance);
                    services.AddSingleton(MatchingStore.Instance);

                    services.AddSingleton<IPlayerFactory, PlayerFactory>();
                    services.AddSingleton<IMatchingFactory, MatchingFactory>();
                    services.AddSingleton<IGameFactory, GameFactory>();
                    services.AddSingleton<IGameResultFactory, GameResultFactory>();

                    services.AddSingleton<IMatchingRepository, MatchingRepository>();
                    services.AddSingleton<IGameRepository, GameRepository>();
                    services.AddSingleton<IResultRepository, GameResultRepository>();

                    services.AddSingleton<IResultCalculator, ResultCalculator>();
                    services.AddSingleton<IGameResultService, GameResultService>();

                    services.AddSingleton<IStartGameUseCase, StartGameUseCase>();
                    services.AddSingleton<ILayPieceUseCase, LayPieceUseCase>();
                    services.AddSingleton<ICreateMatchingUseCase, CreateMatchingUseCase>();
                    services.AddSingleton<ICreateGameUseCase, CreateGameUseCase>();
                })
                .RunConsoleAsync();
        }

        private sealed class PlayerFactory : IPlayerFactory
        {
            public IPlayer Create(string id, string name) => new Player.Impl(id, name);
        }

        private sealed class MatchingFactory : IMatchingFactory
        {
            public IMatching Create(string id, IPlayer first) => new Matching(id, first);
        }

        private sealed class GameFactory : IGameFactory
        {
            public IGame Create(string id, string resultId, string matchingId) => new Game(id, resultId, matchingId);
        }

        private sealed class GameResultFactory : IGameResultFactory
        {
            public IGameResult Create(string id, int darkCount, int lightCount, Winner winner) => new GameResult(id, darkCount, lightCount, winner);
        }
    }
}

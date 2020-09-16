using System.Threading.Tasks;
using Grpc.Core;
using MagicOnion;
using MagicOnion.Hosting;
using MagicOnion.HttpGateway.Swagger;
using MagicOnion.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            var magicOnionHost = MagicOnionHost.CreateDefaultBuilder()
                .UseMagicOnion(
                    new MagicOnionOptions(isReturnExceptionStackTraceInErrorDetail: true)
                    {
                        MagicOnionLogger = new MagicOnionLogToGrpcLoggerWithNamedDataDump()
                    },
                    new ServerPort("0.0.0.0", 12345, ServerCredentials.Insecure))
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(GrpcEnvironment.Logger);

                    // todo 分割しないとやばいことになるので分割する
                    // dependency injection
                    services.AddSingleton(MessageBroker<IResultMessage>.CreateInstance());

                    services.AddSingleton(GameStore.Instance);
                    services.AddSingleton(GameResultStore.Instance);
                    services.AddSingleton(RoomStore.Instance);

                    services.AddSingleton<IPlayerFactory, PlayerFactory>();
                    services.AddSingleton<IRoomFactory, RoomFactory>();
                    services.AddSingleton<IGameFactory, GameFactory>();
                    services.AddSingleton<IGameResultFactory, GameResultFactory>();

                    services.AddSingleton<IRoomRepository, RoomRepository>();
                    services.AddSingleton<IGameRepository, GameRepository>();
                    services.AddSingleton<IResultRepository, GameResultRepository>();

                    services.AddSingleton<IResultCalculator, ResultCalculator>();
                    services.AddSingleton<IGameResultService, GameResultService>();

                    services.AddSingleton<ILayPieceUseCase, LayPieceUseCase>();
                    services.AddSingleton<ICreateRoomUseCase, CreateRoomUseCase>();
                    services.AddSingleton<ICreateGameUseCase, CreateGameUseCase>();
                    services.AddSingleton<IEnterRoomUseCase, EnterRoomUseCase>();
                    services.AddSingleton<IFindRoomUseCase, FindRoomUseCase>();
                    services.AddSingleton<IStartGameUseCase, StartGameUseCase>();
                })
                .UseConsoleLifetime()
                .Build();

            // NuGet: Microsoft.AspNetCore.Server.Kestrel
            var webHost = new WebHostBuilder()
                    .ConfigureServices(collection =>
                    {
                        // Add MagicOnionServiceDefinition for reference from Startup.
                        collection.AddSingleton(magicOnionHost.Services.GetService<MagicOnionHostedServiceDefinition>().ServiceDefinition);
                    })
                    .UseKestrel()
                    .UseStartup<Startup>()
                    .UseUrls("http://0.0.0.0:5432")
                    .Build();

            // Run and wait both.
            await Task.WhenAll(webHost.RunAsync(), magicOnionHost.RunAsync());
        }

        private sealed class PlayerFactory : IPlayerFactory
        {
            public IPlayer Create(string id, string name) => new Player.Impl(id, name);
        }

        private sealed class RoomFactory : IRoomFactory
        {
            public IRoom Create(string id) => new Room(id);
        }

        private sealed class GameFactory : IGameFactory
        {
            public IGame Create(string id, string resultId, IRoom room) => new Game(id, resultId, room);
        }

        private sealed class GameResultFactory : IGameResultFactory
        {
            public IGameResult Create(string id, int darkCount, int lightCount, Winner winner) => new GameResult(id, darkCount, lightCount, winner);
        }
    }

    // WebAPI Startup configuration.
    public class Startup
    {
        // Inject MagicOnionServiceDefinition from DIl
        public void Configure(IApplicationBuilder app, MagicOnionServiceDefinition magicOnion)
        {
            // Optional:Add Summary to Swagger
            // var xmlName = "Sandbox.NetCoreServer.xml";
            // var xmlPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), xmlName);

            // HttpGateway requires two middlewares.
            // One is SwaggerView(MagicOnionSwaggerMiddleware)
            // One is Http1-JSON to gRPC-MagicOnion gateway(MagicOnionHttpGateway)
            app.UseMagicOnionSwagger(magicOnion.MethodHandlers, new SwaggerOptions("MagicOnion.Server", "Swagger Integration Test", "/")
            {
                // XmlDocumentPath = xmlPath
            });
            app.UseMagicOnionHttpGateway(magicOnion.MethodHandlers, new Channel("localhost:12345", ChannelCredentials.Insecure));
        }
    }
}

using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using MagicOnion.Hosting;
using MagicOnion.Server;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Pommel.Server.UseCase.InGame;
using Pommel.Server.Domain.InGame;

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
                    new ServerPort("localhost", 12345, ServerCredentials.Insecure))
                .ConfigureServices((hostContext, services) =>
                {
                    // todo 分割しないとやばいことになるので分割する
                    // dependency injection
                    services.AddSingleton<IStartGameUseCase, StartGameUseCase>();
                    services.AddSingleton<ILayPieceUseCase, LayPieceUseCase>();
                    services.AddSingleton<ICreateMatchingUseCase, CreateMatchingUseCase>();
                    services.AddSingleton<ICreateGameUseCase, CreateGameUseCase>();
                })
                .RunConsoleAsync();
        }
    }
}

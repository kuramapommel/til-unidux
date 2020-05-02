using System;
using System.Collections.Generic;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Presentation.Scene.InGame.Dispatcher;
using Pommel.Reversi.Presentation.Scene.InGame.State;
using Pommel.Reversi.Presentation.Scene.InGame.View;
using Pommel.Reversi.UseCase.InGame;
using Pommel.Reversi.UseCase.InGame.Dto;
using Pommel.Reversi.UseCase.Shared;
using UniRx.Async;
using Zenject;

namespace Pommel.Reversi.Installer.Scene.Ingame
{
    public sealed class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // factories
            Container.BindIFactory<IEnumerable<IStoneState>, IGameBoardState>().To<GameBoardState>().AsCached();
            Container.BindIFactory<Point, Color, IStoneState>().To<StoneState>().AsCached();
            Container.BindIFactory<IGameRepository, IEventPublisher, string, IPutStoneUseCase>().To<PutStoneUseCase>().AsCached();
            Container.BindIFactory<IEventBroker, IEventPublisher>(); // todo concrete class 実装する
            Container.BindIFactory<Func<ResultDto, UniTask>, Func<PuttedDto, UniTask>, IGameResultService, IEventSubscriber>().To<PuttedStoneEventSubscriber>().AsCached();

            // repositories
            Container.BindInterfacesTo<IGameRepository>().AsCached(); // todo concrete class 実装する

            // domain service
            Container.BindInterfacesTo<IGameService>().AsCached(); // todo concrete class 実装する
            Container.BindInterfacesTo<IGameResultService>().AsCached(); // todo concrete class 実装する
            Container.BindInterfacesTo<IEventBroker>().AsCached(); // todo concrete class 実装する

            // view
            Container.BindInterfacesTo<GameBoard>().AsCached();

            // dispatchers
            Container.BindInterfacesTo<StoneDispatcher>().AsCached();
        }
    }
}
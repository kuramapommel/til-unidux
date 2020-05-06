using System;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Infrastructure.Repository.InGame;
using Pommel.Reversi.Infrastructure.Service.InGame;
using Pommel.Reversi.Infrastructure.Service.System;
using Pommel.Reversi.Infrastructure.Store.InGame;
using Pommel.Reversi.Infrastructure.Store.System;
using Pommel.Reversi.Presentation.Model.InGame;
using Pommel.Reversi.Presentation.Model.System;
using Pommel.Reversi.Presentation.View.Title;
using Pommel.Reversi.UseCase.InGame;
using Pommel.Reversi.UseCase.InGame.Dto;
using Pommel.Reversi.UseCase.System;
using UniRx.Async;
using UnityEngine;
using Zenject;
using _Color = Pommel.Reversi.Domain.InGame.Color;



namespace Pommel.Reversi.Installer.Scene.Title
{
    public sealed class TitleInstaller : MonoInstaller
    {
        [SerializeField]
        private TitleTapArea m_titleTapArea;

        public override void InstallBindings()
        {
            // factories
            Container.BindIFactory<string, Point, _Color, ILayPieceUseCase, IPieceModel>().To<PieceModel>().AsCached();
            Container.BindIFactory<string, string, IGame>().To<Game>().AsCached();
            Container.BindIFactory<Func<ResultDto, UniTask>, Func<LaidDto, UniTask>, IGameResultService, IEventSubscriber>().To<LaidPieceEventSubscriber>().AsCached();

            // stores
            Container.Bind<IGameStore>().FromInstance(GameStore.Instance).AsSingle();
            Container.Bind<IGameResultStore>().FromInstance(GameResultStore.Instance).AsSingle();

            // repositories
            Container.BindInterfacesTo<GameRepository>().AsCached();

            // domain services
            Container.BindInterfacesTo<GameResultService>().AsCached();
            Container.BindInterfacesTo<EventPublisher>().AsCached();
            Container.BindInterfacesTo<EventBroker>()
                .AsSingle();

            // usecases
            Container.BindInterfacesTo<CreateGameUseCase>().AsCached();
            Container.BindInterfacesTo<StartGameUseCase>().AsCached();
            Container.BindInterfacesTo<LayPieceUseCase>().AsCached();

            // models
            Container.BindInterfacesTo<GameModel>().AsCached();
            Container.BindInterfacesTo<TransitionModel>().AsSingle();

            // views
            Container.BindInterfacesTo<TitleTapArea>().FromInstance(m_titleTapArea).AsCached();
        }
    }
}
using System;
using System.Collections.Generic;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Infrastructure.Repository.InGame;
using Pommel.Reversi.Infrastructure.Service.InGame;
using Pommel.Reversi.Infrastructure.Service.System;
using Pommel.Reversi.Infrastructure.Store.InGame;
using Pommel.Reversi.Infrastructure.Store.System;
using Pommel.Reversi.Presentation.Scene.InGame.Dispatcher;
using Pommel.Reversi.Presentation.Scene.InGame.State;
using Pommel.Reversi.Presentation.Scene.InGame.View;
using Pommel.Reversi.UseCase.InGame;
using Pommel.Reversi.UseCase.InGame.Dto;
using Pommel.Reversi.UseCase.Shared;
using UniRx.Async;
using UnityEngine;
using Zenject;
using _Color = Pommel.Reversi.Domain.InGame.Color;
using _Stone = Pommel.Reversi.Presentation.Scene.InGame.View.Stone;

namespace Pommel.Reversi.Installer.Scene.Ingame
{
    public sealed class GameInstaller : MonoInstaller
    {
        [SerializeField]
        private _Stone m_stonePrefab;

        [SerializeField]
        private RectTransform m_stoneParent;

        public override void InstallBindings()
        {
            // factories
            Container.BindIFactory<IEnumerable<IStoneState>, IGameBoardState>().To<GameBoardState>().AsCached();
            Container.BindIFactory<Point, _Color, IStoneState>().To<StoneState>().AsCached();
            Container.BindIFactory<IGameRepository, IEventPublisher, string, IPutStoneUseCase>().To<PutStoneUseCase>().AsCached();
            Container.BindIFactory<IEventBroker, IEventPublisher>().To<EventPublisher>();
            Container.BindIFactory<Func<ResultDto, UniTask>, Func<PuttedDto, UniTask>, IGameResultService, IEventSubscriber>().To<PuttedStoneEventSubscriber>().AsCached();
            Container.BindIFactory<IStoneState, IStone>().To<_Stone>()
                .FromComponentInNewPrefab(m_stonePrefab)
                .UnderTransform(m_stoneParent)
                .AsCached();

            // stores
            Container.Bind<IGameStore>().FromInstance(GameStore.Instance).AsSingle();
            Container.Bind<IGameResultStore>().FromInstance(GameResultStore.Instance).AsSingle();

            // repositories
            Container.BindInterfacesTo<GameRepository>().AsCached();

            // domain service
            Container.BindInterfacesTo<GameService>().AsCached();
            Container.BindInterfacesTo<GameResultService>().AsCached();
            Container.BindInterfacesTo<EventBroker>().AsSingle();

            // view
            Container.BindInterfacesTo<GameBoard>().AsCached();

            // dispatchers
            Container.BindInterfacesTo<StoneDispatcher>().AsCached();
        }
    }
}
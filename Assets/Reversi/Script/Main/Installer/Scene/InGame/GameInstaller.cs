using System;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Infrastructure.Repository.InGame;
using Pommel.Reversi.Infrastructure.Service.InGame;
using Pommel.Reversi.Infrastructure.Service.System;
using Pommel.Reversi.Infrastructure.Store.InGame;
using Pommel.Reversi.Infrastructure.Store.System;
using Pommel.Reversi.Presentation.Scene.InGame.Presenter;
using Pommel.Reversi.Presentation.Scene.InGame.State;
using Pommel.Reversi.Presentation.Scene.InGame.View;
using Pommel.Reversi.UseCase.InGame;
using Pommel.Reversi.UseCase.InGame.Dto;
using Pommel.Reversi.UseCase.Shared;
using UniRx.Async;
using UnityEngine;
using Zenject;
using _Color = Pommel.Reversi.Domain.InGame.Color;
using _Piece = Pommel.Reversi.Presentation.Scene.InGame.View.Piece;

namespace Pommel.Reversi.Installer.Scene.Ingame
{
    public sealed class GameInstaller : MonoInstaller
    {
        [SerializeField]
        private _Piece m_piecePrefab;

        [SerializeField]
        private GameBoard m_gameBoard;

        public override void InstallBindings()
        {
            // factories
            Container.BindIFactory<Point, _Color, IPieceState>().To<PieceState>().AsCached();
            Container.BindIFactory<IGameRepository, IEventPublisher, string, ILayPieceUseCase>().To<LayPieceUseCase>().AsCached();
            Container.BindIFactory<Func<ResultDto, UniTask>, Func<LaidDto, UniTask>, IGameResultService, IEventSubscriber>().To<LaidPieceEventSubscriber>().AsCached();
            Container.BindIFactory<IPieceState, IPiece>().To<_Piece>()
                .FromComponentInNewPrefab(m_piecePrefab)
                .UnderTransform(m_gameBoard.GetComponent<RectTransform>())
                .AsCached();

            // stores
            Container.Bind<IGameStore>().FromInstance(GameStore.Instance)
                .AsSingle();
            Container.Bind<IGameResultStore>().FromInstance(GameResultStore.Instance)
                .AsSingle();

            // repositories
            Container.BindInterfacesTo<GameRepository>().AsCached();

            // domain service
            Container.BindInterfacesTo<GameService>().AsCached();
            Container.BindInterfacesTo<GameResultService>().AsCached();
            Container.BindInterfacesTo<EventPublisher>().AsCached();
            Container.BindInterfacesTo<EventBroker>()
                .AsSingle();

            // usecase
            Container.BindInterfacesTo<StartGameUseCase>().AsCached();

            // state
            Container.BindInterfacesTo<GameState>().AsCached();

            // view
            Container.BindInterfacesTo<GameBoard>().FromInstance(m_gameBoard).AsCached();

            // presenters
            Container.BindInterfacesTo<PiecePresenter>().AsCached();
            Container.BindInterfacesTo<GamePresenter>().AsCached();
        }
    }
}
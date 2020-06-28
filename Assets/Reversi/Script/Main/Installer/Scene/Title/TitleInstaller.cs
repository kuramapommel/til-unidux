using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Infrastructure.Repository.InGame;
using Pommel.Reversi.Infrastructure.Service.InGame;
using Pommel.Reversi.Infrastructure.Store.InGame;
using Pommel.Reversi.Presentation.Model.InGame;
using Pommel.Reversi.Presentation.State.InGame;
using Pommel.Reversi.Presentation.View.Title;
using Pommel.Reversi.UseCase.InGame;
using UniRx;
using UnityEngine;
using Zenject;
using _Color = Pommel.Reversi.Domain.InGame.Color;

namespace Pommel.Reversi.Installer.Scene.Title
{
    public sealed class TitleInstaller : MonoInstaller
    {
        [SerializeField]
        private TitleTapArea m_titleTapArea = default;

        public override void InstallBindings()
        {
            // factories
            Container.BindIFactory<string, Point, _Color, IPieceModel, IPieceState>().To<PieceState>().AsCached();
            Container.BindIFactory<string, string, IGame>().To<Game>().AsCached();

            // stores
            Container.Bind<IGameStore>().FromInstance(GameStore.Instance).AsSingle();
            Container.Bind<IGameResultStore>().FromInstance(GameResultStore.Instance).AsSingle();

            // repositories
            Container.BindInterfacesTo<GameRepository>().AsCached();

            // domain services
            Container.BindInterfacesTo<GameResultService>().AsCached();
            Container.Bind<IMessageBroker>().To<LaidPieceMessageBroker>().AsCached();

            // usecases
            Container.BindInterfacesTo<CreateGameUseCase>().AsCached();
            Container.BindInterfacesTo<StartGameUseCase>().AsCached();
            Container.BindInterfacesTo<LayPieceUseCase>().AsCached();

            // models
            Container.BindInterfacesTo<GameModel>().AsCached();
            Container.BindInterfacesTo<PieceModel>().AsSingle();

            // viewmodels
            Container.BindInterfacesTo<GameState>().AsCached();

            // views
            Container.BindInterfacesTo<TitleTapArea>().FromInstance(m_titleTapArea).AsCached();
        }
    }
}
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
            Container.BindInterfacesTo<PieceStateFactory>().AsCached();
            Container.BindInterfacesTo<GameFactory>().AsCached();

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

        private sealed class PieceStateFactory : IPieceStateFactory
        {
            public IPieceState Create(string gameId, Point point, _Color color, IPieceModel pieceModel) => new PieceState(gameId, point, color, pieceModel);
        }

        private sealed class GameFactory : IGameFactory
        {
            public IGame Create(string id, string resultId) => new Game(id, resultId);
        }
    }
}
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Infrastructure.Repository.InGame;
using Pommel.Reversi.Infrastructure.Store.InGame;
using Pommel.Reversi.Presentation.Presenter.Title;
using Pommel.Reversi.Presentation.View.Title;
using Pommel.Reversi.UseCase.InGame;
using UnityEngine;
using Zenject;

namespace Pommel.Reversi.Installer.Scene.Title
{
    public sealed class TitleInstaller : MonoInstaller
    {
        [SerializeField]
        private TitleTapArea m_titleTapArea;

        public override void InstallBindings()
        {
            // factories
            Container.BindIFactory<string, string, IGame>().To<Game>().AsCached();

            // stores
            Container.Bind<IGameStore>().FromInstance(GameStore.Instance).AsSingle();
            Container.Bind<IGameResultStore>().FromInstance(GameResultStore.Instance).AsSingle();

            // repositories
            Container.BindInterfacesTo<GameRepository>().AsCached();

            // usecases
            Container.BindInterfacesTo<CreateGameUseCase>().AsCached();

            // views
            Container.BindInterfacesTo<TitleTapArea>().FromInstance(m_titleTapArea).AsCached();

            // presenters
            Container.BindInterfacesTo<TitlePresenter>().AsCached();
        }
    }
}
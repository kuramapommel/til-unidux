using Unidux.SceneTransition;
using Zenject;

namespace Pommel.Reversi.Installer.Project
{
    public sealed class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            // props
            Container.Bind<IProps>().FromInstance(Unidux.Props).AsSingle();

            // state
            Container.Bind<StateRoot>().FromInstance(Unidux.State).AsSingle();

            // dispatcher
            Container.Bind<IDispatcher>().FromInstance(Unidux.Dispatcher).AsSingle();

            // state as observable creator
            Container.Bind<IStateAsObservableCreator>().FromInstance(Unidux.StateAsObservableCreator).AsSingle();

            // operators
            Container.BindInterfacesTo<Reducks.EntryPoint.Opration>().AsSingle();
            Container.BindInterfacesTo<Reducks.InGame.Opration>().AsSingle();
            Container.BindInterfacesTo<Reducks.Title.Opration>().AsSingle();
            Container.BindInterfacesTo<Reducks.Transition.Operation>().AsSingle();

            // scene config
            Container.Bind<ISceneConfig<Domain.Scene.ValueObjects.Scene, Domain.Scene.ValueObjects.Page>>().To<SceneConfig>().AsSingle();

            // magic onion server client
            Container.Bind<Domain.InGame.IClient>().To<Infrastructure.Client.InGameClient>().AsSingle();
        }
    }
}
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

            // scene config
            Container.Bind<ISceneConfig<Domain.Scene.ValueObjects.Scene, Domain.Scene.ValueObjects.Page>>().To<SceneConfig>().AsSingle();

            // magic onion server client
            Container.Bind<Domain.InGame.IClient>().To<Infrastructure.Client.InGameClient>().AsSingle();

            // operation factories
            Container.BindInterfacesTo<EntryPointOperationFactory>().AsSingle();
            Container.BindInterfacesTo<InGameOperationFactory>().AsSingle();
            Container.BindInterfacesTo<TitleOperationFactory>().AsSingle();
            Container.BindInterfacesTo<TransitionOperationFactory>().AsSingle();
        }

        private sealed class EntryPointOperationFactory : Reducks.EntryPoint.Operation.IFactory
        {
            public Reducks.EntryPoint.IOperation Create() => new Reducks.EntryPoint.Operation.Impl();
        }

        private sealed class InGameOperationFactory : Reducks.InGame.Operation.IFactory
        {
            private readonly IProps m_props;

            private readonly Domain.InGame.IClient m_client;

            public InGameOperationFactory(IProps props, Domain.InGame.IClient client)
            {
                m_props = props;
                m_client = client;
            }

            public Reducks.InGame.IOperation Create() => new Reducks.InGame.Operation.Impl(m_props, m_client);
        }

        private sealed class TitleOperationFactory : Reducks.Title.Operation.IFactory
        {
            private readonly IProps m_props;

            private readonly Domain.InGame.IClient m_client;

            public TitleOperationFactory(IProps props, Domain.InGame.IClient client)
            {
                m_props = props;
                m_client = client;
            }

            public Reducks.Title.IOperation Create() => new Reducks.Title.Operation.Impl(m_props, m_client);
        }

        private sealed class TransitionOperationFactory : Reducks.Transition.Operation.IFactory
        {
            public Reducks.Transition.IOperation Create() => new Reducks.Transition.Operation.Impl();
        }
    }
}
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
            Container.BindInterfacesTo<Reducks.EntryPoint.OperationImpls.LoadTitleOperation>().AsSingle();
            Container.BindInterfacesTo<Reducks.InGame.OperationImpls.PutStoneOperation>().AsSingle();
            Container.BindInterfacesTo<Reducks.InGame.OperationImpls.RefreshAndNextTurnOperation>().AsSingle();
            Container.BindInterfacesTo<Reducks.InGame.OperationImpls.ReturnToTitleOperation>().AsSingle();
            Container.BindInterfacesTo<Reducks.Title.OperationImpls.CreateRoomOperation>().AsSingle();
            Container.BindInterfacesTo<Reducks.Title.OperationImpls.EnterRoomOperation>().AsSingle();
            Container.BindInterfacesTo<Reducks.Title.OperationImpls.OpenGameStartModalOperation>().AsSingle();
            Container.BindInterfacesTo<Reducks.Title.OperationImpls.StartGameOperation>().AsSingle();
            Container.BindInterfacesTo<Reducks.Transition.OperationImpls.AdjustPagesOperation>().AsSingle();
            Container.BindInterfacesTo<Reducks.Transition.OperationImpls.LoadSceneOperation>().AsSingle();
        }
    }
}
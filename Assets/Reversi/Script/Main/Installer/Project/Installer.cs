using Pommel.Reversi.Presentation.Model.System;
using Pommel.Reversi.Presentation.State.System;
using Pommel.Reversi.UseCase.InGame;
using UniRx;
using Zenject;

namespace Pommel.Reversi.Installer.Project
{
    public sealed class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            // domain services
            Container.Bind<IMessageBroker>().To<LaidPieceMessageBroker>().AsSingle();

            // models
            Container.BindInterfacesTo<TransitionModel>().AsSingle();

            // viewmodels
            Container.BindInterfacesTo<TransitionState>().AsSingle();
        }
    }
}
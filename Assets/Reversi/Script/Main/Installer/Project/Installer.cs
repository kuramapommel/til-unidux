using Pommel.Reversi.Presentation.Model.System;
using Pommel.Reversi.Presentation.State.System;
using Zenject;

namespace Pommel.Reversi.Installer.Project
{
    public sealed class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            // models
            Container.BindInterfacesTo<TransitionModel>().AsSingle();

            // viewmodels
            Container.BindInterfacesTo<TransitionState>().AsSingle();
        }
    }
}
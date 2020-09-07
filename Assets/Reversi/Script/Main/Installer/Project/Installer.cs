using Zenject;

namespace Pommel.Reversi.Installer.Project
{
    public sealed class Installer : MonoInstaller
    {
        public override void InstallBindings()
        {
            //// models
            //Container.BindInterfacesTo<TransitionModel>().AsSingle();

            //// viewmodels
            //Container.BindInterfacesTo<TransitionState>().AsSingle();
        }
    }
}
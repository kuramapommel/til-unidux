using Pommel.Reversi.Presentation.View.Base;
using UnityEngine;
using Zenject;

namespace Pommel.Reversi.Installer.Scene.Base
{
    public sealed class BaseInstaller : MonoInstaller
    {
        [SerializeField]
        private EntryPoint m_entryPoint = default;

        public override void InstallBindings()
        {
            // views
            Container.BindInterfacesTo<EntryPoint>().FromInstance(m_entryPoint).AsCached();
        }
    }
}
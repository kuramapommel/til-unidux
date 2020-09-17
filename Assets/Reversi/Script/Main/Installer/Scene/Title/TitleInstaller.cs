using Pommel.Reversi.Components.Title;
using UnityEngine;
using Zenject;

namespace Pommel.Reversi.Installer.Scene.Title
{
    public sealed class TitleInstaller : MonoInstaller
    {
        [SerializeField]
        private TitleTapArea m_titleTapArea = default;

        [SerializeField]
        private GameStartModal m_gameStartModal = default;

        public override void InstallBindings()
        {
            // views
            Container.BindInterfacesTo<TitleTapArea>().FromInstance(m_titleTapArea).AsCached();
            Container.BindInterfacesTo<GameStartModal>().FromInstance(m_gameStartModal).AsCached();
        }
    }
}
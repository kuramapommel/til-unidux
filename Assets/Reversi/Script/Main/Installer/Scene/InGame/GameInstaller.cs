using Pommel.Reversi.Components.InGame;
using Pommel.Reversi.Reducks.InGame;
using UnityEngine;
using Zenject;
using static Pommel.Reversi.Domain.InGame.ValueObjects;

namespace Pommel.Reversi.Installer.Scene.Ingame
{
    public sealed class GameInstaller : MonoInstaller
    {
        [SerializeField]
        private Piece m_piecePrefab = default;

        [SerializeField]
        private GameObject m_gameBoard = default;

        [SerializeField]
        private ResultMessage m_resultMessage = default;

        [SerializeField]
        private PlayerInfo m_firstPlayerInfo = default;

        [SerializeField]
        private PlayerInfo m_secondPlayerInfo = default;

        public override void InstallBindings()
        {
            // factories
            Container.BindIFactory<IOperation, IStateAsObservableCreator, Point, IPiece>().To<Piece>()
                .FromComponentInNewPrefab(m_piecePrefab)
                .UnderTransform(m_gameBoard.GetComponent<RectTransform>())
                .AsCached();

            // views
            Container.BindInterfacesTo<ResultMessage>().FromInstance(m_resultMessage).AsCached();
            Container.Bind<IFirstPlayerInfo>().FromInstance(m_firstPlayerInfo).AsCached();
            Container.Bind<ISecondPlayerInfo>().FromInstance(m_secondPlayerInfo).AsCached();
        }
    }
}
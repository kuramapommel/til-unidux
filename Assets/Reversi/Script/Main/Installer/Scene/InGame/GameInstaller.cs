using Pommel.Reversi.Infrastructure.Service.InGame;
using Pommel.Reversi.Presentation.Model.InGame;
using Pommel.Reversi.Presentation.View.InGame;
using UnityEngine;
using Zenject;
using _Piece = Pommel.Reversi.Presentation.View.InGame.Piece;

namespace Pommel.Reversi.Installer.Scene.Ingame
{
    public sealed class GameInstaller : MonoInstaller
    {
        [SerializeField]
        private _Piece m_piecePrefab;

        [SerializeField]
        private GameBoard m_gameBoard;

        [SerializeField]
        private ResultMessage m_resultMessage;

        public override void InstallBindings()
        {
            // factories
            Container.BindIFactory<IPieceModel, IPiece>().To<_Piece>()
                .FromComponentInNewPrefab(m_piecePrefab)
                .UnderTransform(m_gameBoard.GetComponent<RectTransform>())
                .AsCached();

            // domain services
            Container.BindInterfacesTo<GameService>().AsCached();

            // views
            Container.BindInterfacesTo<GameBoard>().FromInstance(m_gameBoard).AsCached();
            Container.BindInterfacesTo<ResultMessage>().FromInstance(m_resultMessage).AsCached();
        }
    }
}
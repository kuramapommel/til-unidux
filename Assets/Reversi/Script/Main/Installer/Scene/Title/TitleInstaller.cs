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
            // api client
            //Container.Bind<IInGameClient>().To<InGameClient>().AsCached().NonLazy();

            // factories
            //Container.BindInterfacesTo<PieceStateFactory>().AsCached();
            //Container.BindInterfacesTo<GameFactory>().AsCached();
            //Container.BindInterfacesTo<PlayerStateFactory>().AsCached();
            //Container.BindInterfacesTo<PlayerFactory>().AsCached();
            //Container.BindInterfacesTo<MatchingFactory>().AsCached();

            // models
            //Container.Bind<IGameModel>().To<GameModel>().AsCached();
            //Container.Bind<IPieceModel>().To<PieceModel>().AsSingle();

            // viewmodels
            //Container.Bind<IGameViewModel>().To<GameViewModel>().AsCached();
            //Container.BindInterfacesTo<TitleViewModel>().AsCached();

            // views
            Container.BindInterfacesTo<TitleTapArea>().FromInstance(m_titleTapArea).AsCached();
            Container.BindInterfacesTo<GameStartModal>().FromInstance(m_gameStartModal).AsCached();
        }

        //private sealed class PieceStateFactory : IPieceStateFactory
        //{
        //    public IPieceViewModel Create(string gameId, Point point, _Color color, IPieceModel pieceModel) => new PieceViewModel(gameId, point, color, pieceModel);
        //}

        //private sealed class GameFactory : IGameFactory
        //{
        //    public IGame Create(string id, string nextTurnPlayerId, IEnumerable<Piece> pieces) => new Game(id, nextTurnPlayerId, pieces);
        //}

        //private sealed class PlayerStateFactory : IPlayerStateFactory
        //{
        //    public IPlayerViewModel Create(string playerId, string name, bool isTurnPlayer) => new PlayerViewModel(playerId, name, isTurnPlayer);
        //}

        //private sealed class PlayerFactory : IPlayerFactory
        //{
        //    public IPlayer Create(string id, string name) => new Player.Impl(id, name);
        //}

        //private sealed class MatchingFactory : IMatchingFactory
        //{
        //    public IMatching Create(string id, IPlayer first = default, IPlayer second = default) => new Matching(id, first, second);
        //}
    }
}
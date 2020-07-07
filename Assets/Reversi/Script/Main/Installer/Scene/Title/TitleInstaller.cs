using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Infrastructure.Networking.Client;
using Pommel.Reversi.Infrastructure.Repository.InGame;
using Pommel.Reversi.Infrastructure.Service.InGame;
using Pommel.Reversi.Infrastructure.Store.InGame;
using Pommel.Reversi.Presentation.Model.InGame;
using Pommel.Reversi.Presentation.State.InGame;
using Pommel.Reversi.Presentation.View.Title;
using Pommel.Reversi.UseCase.InGame;
using UnityEngine;
using Zenject;
using _Color = Pommel.Reversi.Domain.InGame.Color;
using _LaidPieceMessageBroker = Pommel.Reversi.Infrastructure.Service.InGame.LaidPieceMessageBroker;

namespace Pommel.Reversi.Installer.Scene.Title
{
    public sealed class TitleInstaller : MonoInstaller
    {
        [SerializeField]
        private TitleTapArea m_titleTapArea = default;

        public override void InstallBindings()
        {
            // factories
            Container.BindInterfacesTo<PieceStateFactory>().AsCached();
            Container.BindInterfacesTo<GameFactory>().AsCached();
            Container.BindInterfacesTo<PlayerStateFactory>().AsCached();
            Container.BindInterfacesTo<PlayerFactory>().AsCached();
            Container.BindInterfacesTo<MatchingFactory>().AsCached();
            Container.BindInterfacesTo<InGameClientFactory>().AsCached();

            // stores
            Container.Bind<IGameStore>().FromInstance(GameStore.Instance).AsSingle();
            Container.Bind<IGameResultStore>().FromInstance(GameResultStore.Instance).AsSingle();
            Container.Bind<ILaidResultStore>().FromInstance(LaidResultStore.Instance).AsSingle();
            Container.Bind<IMatchingStore>().FromInstance(MatchingStore.Instance).AsSingle();

            // repositories
            Container.BindInterfacesTo<GameRepository>().AsCached();
            Container.BindInterfacesTo<MatchingRepository>().AsCached();

            // domain services
            Container.BindInterfacesTo<GameResultService>().AsCached();
            Container.BindInterfacesTo<LaidResultService>().AsCached();
            Container.BindInterfacesTo<_LaidPieceMessageBroker>().AsCached();

            // usecases
            Container.BindInterfacesTo<CreateMatchingUseCase>().AsCached();
            Container.BindInterfacesTo<EntryMatchingUseCase>().AsCached();
            Container.BindInterfacesTo<CreateGameUseCase>().AsCached();
            Container.BindInterfacesTo<StartGameUseCase>().AsCached();
            Container.BindInterfacesTo<LayPieceUseCase>().AsCached();

            // models
            Container.BindInterfacesTo<GameModel>().AsCached();
            Container.BindInterfacesTo<PieceModel>().AsSingle();

            // viewmodels
            Container.BindInterfacesTo<GameState>().AsCached();

            // views
            Container.BindInterfacesTo<TitleTapArea>().FromInstance(m_titleTapArea).AsCached();
        }

        private sealed class PieceStateFactory : IPieceStateFactory
        {
            public IPieceState Create(string gameId, Point point, _Color color, IPieceModel pieceModel) => new PieceState(gameId, point, color, pieceModel);
        }

        private sealed class GameFactory : IGameFactory
        {
            public IGame Create(string id, string resultId, string matchingId) => new Game(id, resultId, matchingId);
        }

        private sealed class PlayerStateFactory : IPlayerStateFactory
        {
            public IPlayerState Create(string playerId, string name, bool isTurnPlayer) => new PlayerState(playerId, name, isTurnPlayer);
        }

        private sealed class PlayerFactory : IPlayerFactory
        {
            public IPlayer Create(string id, string name) => new Player.Impl(id, name);
        }

        private sealed class MatchingFactory : IMatchingFactory
        {
            public IMatching Create(string id, IPlayer first) => new Matching(id, first);
        }

        private sealed class InGameClientFactory : IInGameClientFactory
        {
            public IInGameClient Create() => new InGameClient();
        }
    }
}
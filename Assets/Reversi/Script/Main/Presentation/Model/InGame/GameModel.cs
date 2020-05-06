using System;
using System.Collections.Generic;
using System.Linq;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.UseCase.InGame;
using Pommel.Reversi.UseCase.InGame.Dto;
using Pommel.Reversi.UseCase.Shared;
using UniRx;
using UniRx.Async;
using Zenject;

namespace Pommel.Reversi.Presentation.Model.InGame
{
    public interface IGameModel
    {
        IEnumerable<IPieceModel> PieceModels { get; }

        IObservable<IGame> OnStart { get; }

        IObservable<Winner> Winner { get; }

        UniTask<IGame> CreateGameAsync();

        UniTask Start();

        UniTask Finish(Winner winner);

        UniTask Refresh(IEnumerable<Piece> pieces);
    }

    public sealed class GameModel : IGameModel
    {
        private readonly IFactory<string, Point, Color, ILayPieceUseCase, IPieceModel> m_pieceModelFactory;

        private readonly IFactory<Func<ResultDto, UniTask>, Func<LaidDto, UniTask>, IGameResultService, IEventSubscriber> m_eventSubscriberFactory;

        private readonly IGameResultService m_gameResultService;

        private readonly IEventBroker m_eventBroker;

        private readonly ICreateGameUseCase m_createGameUseCase;

        private readonly IStartGameUseCase m_startGameUseCase;

        private readonly ILayPieceUseCase m_layPieceUseCase;

        private readonly IList<IPieceModel> m_pieceModels = new List<IPieceModel>();

        private readonly ISubject<IGame> m_onStart = new Subject<IGame>();

        private readonly IReactiveProperty<Winner> m_winner = new ReactiveProperty<Winner>();

        public IEnumerable<IPieceModel> PieceModels => m_pieceModels;

        public IObservable<IGame> OnStart => m_onStart;

        public IObservable<Winner> Winner => m_winner;

        public GameModel(
            IFactory<string, Point, Color, ILayPieceUseCase, IPieceModel> pieceModelFactory,
            IFactory<Func<ResultDto, UniTask>, Func<LaidDto, UniTask>, IGameResultService, IEventSubscriber> eventSubscriberFactory,
            IGameResultService gameResultService,
            IEventBroker eventBroker,
            ICreateGameUseCase createGameUseCase,
            IStartGameUseCase startGameUseCase,
            ILayPieceUseCase layPieceUseCase
            )
        {
            m_pieceModelFactory = pieceModelFactory;
            m_eventSubscriberFactory = eventSubscriberFactory;
            m_gameResultService = gameResultService;
            m_eventBroker = eventBroker;
            m_createGameUseCase = createGameUseCase;
            m_startGameUseCase = startGameUseCase;
            m_layPieceUseCase = layPieceUseCase;

            m_eventBroker.RegisterSubscriber<ILaidPieceEvent>(
                m_eventSubscriberFactory.Create(result => Finish(result.Winner), game => Refresh(game.Pieces), m_gameResultService)
                );
        }

        public UniTask<IGame> CreateGameAsync() => m_createGameUseCase.Execute();

        public async UniTask Start()
        {
            var game = await m_startGameUseCase.Execute();
            foreach (var pieceModel in game.Pieces.Select(piece => m_pieceModelFactory.Create(
                game.Id,
                piece.Point,
                piece.Color,
                m_layPieceUseCase)))
            {
                m_pieceModels.Add(pieceModel);
            }
            m_onStart.OnNext(game);
            m_onStart.OnCompleted();
        }

        public async UniTask Finish(Winner winner) => m_winner.Value = winner;

        public async UniTask Refresh(IEnumerable<Piece> pieces)
        {
            foreach (var (piece, model) in pieces
                .Join(
                    m_pieceModels,
                    pieceEntity => pieceEntity.Point,
                    model => model.Point,
                    (pieceEntity, model) => (pieceEntity, model)
                ))
            {
                model.SetColor(piece.Color.Convert());
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.UseCase.InGame;
using UniRx;
using UniRx.Async;
using Zenject;
using static Pommel.Reversi.Domain.InGame.Winner;

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

        private readonly IGameResultService m_gameResultService;

        private readonly ICreateGameUseCase m_createGameUseCase;

        private readonly IStartGameUseCase m_startGameUseCase;

        private readonly ILayPieceUseCase m_layPieceUseCase;

        private readonly IList<IPieceModel> m_pieceModels = new List<IPieceModel>();

        private readonly ISubject<IGame> m_onStart = new Subject<IGame>();

        private readonly IReactiveProperty<Winner> m_winner = new ReactiveProperty<Winner>(Undecided);

        private readonly IMessageReceiver m_messageReceiver;

        public IEnumerable<IPieceModel> PieceModels => m_pieceModels;

        public IObservable<IGame> OnStart => m_onStart;

        public IObservable<Winner> Winner => m_winner;

        public GameModel(
            IFactory<string, Point, Color, ILayPieceUseCase, IPieceModel> pieceModelFactory,
            IGameResultService gameResultService,
            ICreateGameUseCase createGameUseCase,
            IStartGameUseCase startGameUseCase,
            ILayPieceUseCase layPieceUseCase,
            IMessageBroker messageReceiver
            )
        {
            m_pieceModelFactory = pieceModelFactory;
            m_gameResultService = gameResultService;
            m_createGameUseCase = createGameUseCase;
            m_startGameUseCase = startGameUseCase;
            m_layPieceUseCase = layPieceUseCase;
            m_messageReceiver = messageReceiver;
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

            m_messageReceiver.Receive<ILaidPieceEvent>()
                .Where(message => message.Game.State == State.GameSet)
                .SelectMany(message => m_gameResultService.FindById(message.Game.ResultId).ToObservable())
                .Subscribe(result => Finish(result.Winner)); // todo add IDisposable

            m_messageReceiver.Receive<ILaidPieceEvent>()
                .Subscribe(message => Refresh(message.Game.Pieces)); // todo add IDisposable

            m_onStart.OnNext(game);
            m_onStart.OnCompleted();
        }

        public UniTask Finish(Winner winner)
        {
            m_winner.Value = winner;
            return UniTask.CompletedTask;
        }

        public UniTask Refresh(IEnumerable<Piece> pieces)
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

            return UniTask.CompletedTask;
        }
    }
}
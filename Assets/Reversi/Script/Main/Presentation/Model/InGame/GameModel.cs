using System;
using System.Collections.Generic;
using System.Linq;
using Pommel.Reversi.Domain.InGame;
using UniRx;
using Zenject;

namespace Pommel.Reversi.Presentation.Model.InGame
{
    public interface IGameModel
    {
        IEnumerable<IPieceModel> PieceModels { get; }

        IObservable<Unit> OnStart { get; }

        IObservable<Winner> Winner { get; } 

        void Start(IEnumerable<Piece> pieces);

        void Finish(Winner winner);

        void Refresh(IEnumerable<Piece> pieces);
    }

    public sealed class GameModel : IGameModel
    {
        private readonly IFactory<Point, Color, IPieceModel> m_pieceModelFactory;

        private readonly IList<IPieceModel> m_pieceModels = new List<IPieceModel>();

        private readonly ISubject<Unit> m_onStart = new Subject<Unit>();

        private readonly IReactiveProperty<Winner> m_winner = new ReactiveProperty<Winner>();

        public IEnumerable<IPieceModel> PieceModels => m_pieceModels;

        public IObservable<Unit> OnStart => m_onStart;

        public IObservable<Winner> Winner => m_winner.SkipLatestValueOnSubscribe();

        public GameModel(IFactory<Point, Color, IPieceModel> pieceModelFactory)
        {
            m_pieceModelFactory = pieceModelFactory;
        }

        public void Start(IEnumerable<Piece> pieces)
        {
            foreach (var pieceModel in pieces.Select(piece => m_pieceModelFactory.Create(piece.Point, piece.Color)))
            {
                m_pieceModels.Add(pieceModel);
            }
            m_onStart.OnNext(Unit.Default);
            m_onStart.OnCompleted();
        }

        public void Finish(Winner winner) => m_winner.Value = winner;

        public void Refresh(IEnumerable<Piece> pieces)
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
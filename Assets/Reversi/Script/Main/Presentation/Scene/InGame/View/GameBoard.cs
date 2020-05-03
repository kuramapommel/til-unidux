using System;
using System.Collections.Generic;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Presentation.Scene.InGame.State;
using UniRx;
using UnityEngine;
using Zenject;
using _Piece = Pommel.Reversi.Domain.InGame.Piece;

namespace Pommel.Reversi.Presentation.Scene.InGame.View
{
    public interface IGameBoard
    {
        void InstantiatePieces(IGameState state, Func<Point, IObservable<IEnumerable<_Piece>>> layPieceAsync);
    }

    [RequireComponent(typeof(RectTransform))]
    public sealed class GameBoard : MonoBehaviour, IGameBoard
    {
        private IFactory<IPieceState, IPiece> m_pieceFactory;

        private IList<IPiece> Pieces { get; } = new List<IPiece>();

        [Inject]
        public void Construct(IFactory<IPieceState, IPiece> pieceFactory)
        {
            m_pieceFactory = pieceFactory;
        }

        public void InstantiatePieces(IGameState state, Func<Point, IObservable<IEnumerable<_Piece>>> layPieceAsync)
        {
            foreach (var pieceState in state.PieceStates)
            {
                var piece = m_pieceFactory.Create(pieceState);
                piece.OnLayAsObservable()
                    .ContinueWith(layPieceAsync(pieceState.Point))
                    .Subscribe(_ => { }, Debug.Log);
                Pieces.Add(piece);
            }
        }
    }
}
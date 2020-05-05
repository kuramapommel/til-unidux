using System;
using System.Collections.Generic;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Presentation.Model.InGame;
using UniRx;
using UniRx.Async;
using UnityEngine;
using Zenject;

namespace Pommel.Reversi.Presentation.View.InGame
{
    public interface IGameBoard
    {
        void InstantiatePieces(IGameModel model, Func<int, int, UniTask<IGame>> layPiece);
    }

    [RequireComponent(typeof(RectTransform))]
    public sealed class GameBoard : MonoBehaviour, IGameBoard
    {
        private IFactory<IPieceModel, IPiece> m_pieceFactory;

        private IList<IPiece> Pieces { get; } = new List<IPiece>();

        [Inject]
        public void Construct(IFactory<IPieceModel, IPiece> pieceFactory)
        {
            m_pieceFactory = pieceFactory;
        }

        public void InstantiatePieces(IGameModel model, Func<int, int, UniTask<IGame>> layPiece)
        {
            foreach (var pieceModel in model.PieceModels)
            {
                var piece = m_pieceFactory.Create(pieceModel);
                piece.OnLayAsObservable
                    .Subscribe(_ => layPiece(pieceModel.Point.X, pieceModel.Point.Y), Debug.Log);
                Pieces.Add(piece);
            }
        }
    }
}
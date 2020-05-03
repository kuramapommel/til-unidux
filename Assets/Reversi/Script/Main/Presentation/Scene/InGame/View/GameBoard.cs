using System;
using System.Collections.Generic;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Presentation.Scene.InGame.State;
using UniRx;
using UniRx.Async;
using UnityEngine;
using Zenject;

namespace Pommel.Reversi.Presentation.Scene.InGame.View
{
    public interface IGameBoard
    {
        void InstantiatePieces(IGameState state, Func<int, int, UniTask<IGame>> layPiece);
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

        public void InstantiatePieces(IGameState state, Func<int, int, UniTask<IGame>> layPiece)
        {
            foreach (var pieceState in state.PieceStates)
            {
                var piece = m_pieceFactory.Create(pieceState);
                piece.OnLayAsObservable
                    .Subscribe(_ => layPiece(pieceState.Point.X, pieceState.Point.Y), Debug.Log);
                Pieces.Add(piece);
            }
        }
    }
}
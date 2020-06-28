using System.Collections.Generic;
using Pommel.Reversi.Presentation.State.InGame;
using UniRx;
using UniRx.Async;
using UnityEngine;
using Zenject;

namespace Pommel.Reversi.Presentation.View.InGame
{
    public interface IGameBoard
    {
    }

    [RequireComponent(typeof(RectTransform))]
    public sealed class GameBoard : MonoBehaviour, IGameBoard
    {
        private IList<IPiece> Pieces { get; } = new List<IPiece>();

        [Inject]
        public void Construct(IFactory<IPieceState, IPiece> pieceFactory, IGameState gameState)
        {
            gameState.OnStart
                .Subscribe(_ =>
                {
                    foreach (var pieceState in gameState.PieceStates)
                    {
                        var piece = pieceFactory.Create(pieceState);
                        Pieces.Add(piece);
                    }
                },
                UnityEngine.Debug.Log);

            gameState.Start().AsUniTask().Forget();
        }
    }
}
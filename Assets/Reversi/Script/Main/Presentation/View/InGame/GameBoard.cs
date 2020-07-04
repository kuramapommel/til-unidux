using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pommel.Reversi.Presentation.State.InGame;
using UniRx;
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
                .TakeUntilDestroy(this)
                .Subscribe(_ =>
                {
                    foreach (var piece in gameState.PieceStates.Select(pieceFactory.Create)) Pieces.Add(piece);
                },
                UnityEngine.Debug.Log);

            gameState.Start().AsUniTask().Forget();
        }
    }
}
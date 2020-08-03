using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pommel.Reversi.Presentation.ViewModel.InGame;
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
        public void Construct(IFactory<IPieceViewModel, IPiece> pieceFactory, IGameViewModel gameState)
        {
            gameState.OnStart
                .TakeUntilDestroy(this)
                .Subscribe(_ =>
                {
                    foreach (var piece in gameState.PieceStates.Select(pieceFactory.Create)) Pieces.Add(piece);
                },
                UnityEngine.Debug.Log);
        }
    }
}
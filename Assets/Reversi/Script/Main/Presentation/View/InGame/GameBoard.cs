using System.Collections.Generic;
using Pommel.Reversi.Presentation.Model.InGame;
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
        public void Construct(IFactory<IPieceModel, IPiece> pieceFactory, IGameModel gameModel)
        {
            gameModel.OnStart
                .Subscribe(_ =>
                {
                    foreach (var pieceModel in gameModel.PieceModels)
                    {
                        var piece = pieceFactory.Create(pieceModel);
                        Pieces.Add(piece);
                    }
                },
                UnityEngine.Debug.Log);
        }
    }
}
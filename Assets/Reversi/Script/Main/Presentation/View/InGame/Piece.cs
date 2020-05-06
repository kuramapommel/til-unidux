using System;
using Pommel.Reversi.Presentation.Model.InGame;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Pommel.Reversi.Presentation.View.InGame
{
    public interface IPiece
    {
        IObservable<Unit> OnLayAsObservable { get; }
    }

    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public sealed class Piece : MonoBehaviour, IPiece
    {
        private Image m_image;

        public IObservable<Unit> OnLayAsObservable { get; private set; }

        [Inject]
        public void Construct(IPieceModel pieceModel)
        {
            GetComponent<Button>().OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ => pieceModel.Lay());
            m_image = GetComponent<Image>();

            pieceModel.Color.Subscribe(color => m_image.color = color);
        }

        public sealed class Factory : PlaceholderFactory<IPiece>
        {

        }
    }
}
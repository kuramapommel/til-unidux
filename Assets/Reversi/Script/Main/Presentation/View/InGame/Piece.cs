using System;
using Pommel.Reversi.Presentation.Scene.InGame.State;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Pommel.Reversi.Presentation.Scene.InGame.View
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
        public void Construct(IPieceState pieceState)
        {
            var button = GetComponent<Button>();
            OnLayAsObservable = button
                .OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Publish()
                .RefCount();
            m_image = GetComponent<Image>();

            pieceState.Color.Subscribe(color => m_image.color = color);
        }
    }
}
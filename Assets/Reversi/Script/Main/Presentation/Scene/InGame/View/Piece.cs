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
        IObservable<Unit> OnLayAsObservable();
    }

    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public sealed class Piece : MonoBehaviour, IPiece
    {
        private Button m_button;

        private Image m_image;

        public IObservable<Unit> OnLayAsObservable() => m_button.OnClickAsObservable().TakeUntilDestroy(this);

        [Inject]
        public void Construct(IPieceState pieceState)
        {
            m_button = GetComponent<Button>();
            m_image = GetComponent<Image>();

            pieceState.Color.Subscribe(color => m_image.color = color);
        }
    }
}
using Pommel.Reversi.Presentation.State.InGame;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Pommel.Reversi.Presentation.View.InGame
{
    public interface IPiece
    {
    }

    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public sealed class Piece : MonoBehaviour, IPiece
    {
        private Image m_image;

        [Inject]
        public void Construct(IPieceState pieceState)
        {
            GetComponent<Button>().OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ => pieceState.Lay(), Debug.Log);
            m_image = GetComponent<Image>();

            pieceState.Color.Subscribe(color => m_image.color = color);
        }
    }
}
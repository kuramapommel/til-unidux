using System;
using Pommel.Reversi.Reducks.InGame;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static Pommel.Reversi.Reducks.InGame.Selectors;

namespace Pommel.Reversi.Components.InGame
{
    public interface IPiece
    {
    }

    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public sealed class Piece : MonoBehaviour, IPiece
    {
        private Image m_image;

        private ValueObject.Point Point { get; set; }

        [Inject]
        public void Construct(
            IOperation operation,
            IStateAsObservableCreator observableCreator
            )
        {
            m_image = GetComponent<Image>();

            GetComponent<Button>().OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ => operation.PutStone(Point));

            observableCreator.Create(this, state => GetStone(state, Point))
                .Subscribe(stone => m_image.color = stone.StoneColor.Convert());
        }
    }

    public static class ColorEnumExt
    {
        private static readonly Lazy<Color> none = new Lazy<Color>(() => ColorUtility.TryParseHtmlString("#13E70E", out var none)
                ? none
                : default);

        public static Color Convert(this ValueObject.Stone.Color source)
        {
            switch (source)
            {
                case ValueObject.Stone.Color.None: return none.Value;
                case ValueObject.Stone.Color.Dark: return Color.black;
                case ValueObject.Stone.Color.Light: return Color.white;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
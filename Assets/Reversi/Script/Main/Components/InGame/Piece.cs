using System;
using Pommel.Reversi.Domain.InGame;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static Pommel.Reversi.Reducks.InGame.Operations;
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

        private ValueObjects.Point m_point;

        [Inject]
        public void Construct(
            IStateAsObservableCreator observableCreator,
            ValueObjects.Point point,
            IDispatcher dispatcher,
            IPutableStone putableStone
            )
        {
            m_image = GetComponent<Image>();
            m_point = point;

            GetComponent<Button>().OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ => dispatcher.Dispatch(putableStone.PutStone(m_point)));

            observableCreator.Create(this, state => state.InGame.Board.IsStateChanged, state => GetStone(state, m_point))
                .Subscribe(stone => m_image.color = stone.StoneColor.Convert());
        }
    }

    public static class ColorEnumExt
    {
        private static readonly Lazy<Color> none = new Lazy<Color>(() => ColorUtility.TryParseHtmlString("#13E70E", out var none)
                ? none
                : default);

        public static Color Convert(this ValueObjects.Stone.Color source)
        {
            switch (source)
            {
                case ValueObjects.Stone.Color.None: return none.Value;
                case ValueObjects.Stone.Color.Dark: return Color.black;
                case ValueObjects.Stone.Color.Light: return Color.white;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}
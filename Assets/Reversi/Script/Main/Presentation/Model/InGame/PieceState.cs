using System;
using Pommel.Reversi.Domain.InGame;
using UniRx;
using UnityEngine;
using _Color = UnityEngine.Color;
using _ColorEnum = Pommel.Reversi.Domain.InGame.Color;

namespace Pommel.Reversi.Presentation.Model.InGame
{
    public interface IPieceModel
    {
        Point Point { get; }

        IReadOnlyReactiveProperty<_Color> Color { get; }

        void SetColor(_Color color);
    }

    public sealed class PieceModel : IPieceModel
    {
        private readonly IReactiveProperty<_Color> m_color;

        public Point Point { get; }

        public IReadOnlyReactiveProperty<_Color> Color => m_color;

        public PieceModel(Point point, _ColorEnum color)
        {
            Point = point;
            m_color = new ReactiveProperty<_Color>(color.Convert());
        }

        public void SetColor(_Color color) => m_color.Value = color;
    }

    public static class ColorEnumExt
    {
        private static readonly Lazy<_Color> none = new Lazy<_Color>(() => ColorUtility.TryParseHtmlString("#13E70E", out var none)
                ? none
                : default);

        public static _Color Convert(this _ColorEnum source)
        {
            switch (source)
            {
                case _ColorEnum.None: return none.Value;
                case _ColorEnum.Dark: return _Color.black;
                case _ColorEnum.Light: return _Color.white;
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}
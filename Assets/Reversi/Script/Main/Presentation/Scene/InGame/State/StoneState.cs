using System;
using Pommel.Reversi.Domain.InGame;
using UniRx;
using UnityEngine;
using _Color = UnityEngine.Color;
using _ColorEnum = Pommel.Reversi.Domain.InGame.Color;

namespace Pommel.Reversi.Presentation.Scene.InGame.State
{
    public interface IStoneState
    {
        Point Point { get; }

        IReadOnlyReactiveProperty<_Color> Color { get; }

        void SetColor(_Color color); 
    }

    public sealed class StoneState : IStoneState
    {
        private readonly IReactiveProperty<_Color> m_color = new ReactiveProperty<_Color>();

        public Point Point { get; }

        public IReadOnlyReactiveProperty<_Color> Color => m_color;

        public void SetColor(_Color color) => m_color.Value = color;

        public StoneState(Point point, _ColorEnum color)
        {
            Point = point;
            m_color.Value = color.Convert();
        }
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
                case _ColorEnum.Black: return _Color.black;
                case _ColorEnum.White: return _Color.white;
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}
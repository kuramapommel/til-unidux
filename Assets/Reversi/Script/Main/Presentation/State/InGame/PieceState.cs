using System;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.UseCase.InGame;
using UniRx;
using UniRx.Async;
using UnityEngine;
using _Color = UnityEngine.Color;
using _ColorEnum = Pommel.Reversi.Domain.InGame.Color;

namespace Pommel.Reversi.Presentation.State.InGame
{
    public interface IPieceState
    {
        Point Point { get; }

        IReadOnlyReactiveProperty<_Color> Color { get; }

        void SetColor(_Color color);

        UniTask<IGame> Lay();
    }

    public sealed class PieceState : IPieceState
    {
        private readonly string m_gameId;

        private readonly IReactiveProperty<_Color> m_color;

        private readonly ILayPieceUseCase m_layPieceUseCase;

        public Point Point { get; }

        public IReadOnlyReactiveProperty<_Color> Color => m_color;

        public PieceState(string gameId, Point point, _ColorEnum color, ILayPieceUseCase layPieceUseCase)
        {
            m_gameId = gameId;
            Point = point;
            m_color = new ReactiveProperty<_Color>(color.Convert());
            m_layPieceUseCase = layPieceUseCase;
        }

        public void SetColor(_Color color) => m_color.Value = color;

        public UniTask<IGame> Lay() => m_layPieceUseCase.Execute(m_gameId, Point.X, Point.Y);
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
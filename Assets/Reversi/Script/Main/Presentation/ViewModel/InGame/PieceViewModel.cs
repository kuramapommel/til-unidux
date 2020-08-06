using System;
using System.Threading.Tasks;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Presentation.Model.InGame;
using UniRx;
using UnityEngine;
using _Color = UnityEngine.Color;
using _ColorEnum = Pommel.Reversi.Domain.InGame.Color;

namespace Pommel.Reversi.Presentation.ViewModel.InGame
{
    public interface IPieceViewModel : IDisposable
    {
        Point Point { get; }

        IReadOnlyReactiveProperty<_Color> Color { get; }

        void SetColor(_Color color);

        Task Lay();
    }

    public interface IPieceStateFactory
    {
        IPieceViewModel Create(string gameId, Point point, _ColorEnum color, IPieceModel pieceModel);
    }

    public sealed class PieceViewModel : IPieceViewModel
    {
        private readonly IPieceModel m_pieceModel;

        private readonly string m_gameId;

        private readonly IReactiveProperty<_Color> m_color = new ReactiveProperty<_Color>();

        public Point Point { get; }

        public IReadOnlyReactiveProperty<_Color> Color => m_color;

        private readonly CompositeDisposable m_disposables = new CompositeDisposable();

        public PieceViewModel(string gameId, Point point, _ColorEnum color, IPieceModel pieceModel)
        {
            m_gameId = gameId;
            Point = point;
            m_color.Value = color.Convert();
            m_pieceModel = pieceModel;

            pieceModel.AddTo(m_disposables);
        }

        public void SetColor(_Color color) => m_color.Value = color;

        public async Task Lay() => await m_pieceModel.LayPiece(m_gameId, Point.X, Point.Y);

        void IDisposable.Dispose()
        {
            m_disposables.Dispose();
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
                case _ColorEnum.Dark: return _Color.black;
                case _ColorEnum.Light: return _Color.white;
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}
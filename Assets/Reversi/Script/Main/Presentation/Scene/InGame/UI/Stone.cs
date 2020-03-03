using UnityEngine;
using UnityEngine.UI;
using System;
using Pommel.Reversi.Presentation.Scene.InGame.Dispatcher;

namespace Pommel.Reversi.Presentation.Scene.InGame.UI
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public sealed class Stone : MonoBehaviour, IDisposable
    {
        [SerializeField]
        private Button m_button;

        [SerializeField]
        private Image m_image;

        private IDisposable m_stoneDisposable;

        public void TurnBlack() => m_image.color = Color.black;

        public void TurnWhite() => m_image.color = Color.white;

        public void None() => m_image.color = "#13E70E".ToColor();

        public bool IsBlack => m_image.color == Color.black;

        public bool IsWhite => m_image.color == Color.white;

        public bool IsNone => m_image.color == "#13E70E".ToColor();

        public Button Button => m_button;

        public Image Image => Image;

        public void Initialize(int x, int y)
        {
            m_stoneDisposable = this.ApplyPutEvent(x, y);
        }

        public void Dispose()
        {
            m_stoneDisposable.Dispose();
            m_stoneDisposable = null;
        }
    }

    public static class StringExtension
    {
        public static Color ToColor(this string self) =>
            ColorUtility.TryParseHtmlString(self, out var color)
                ? color
                : default;
    }
}

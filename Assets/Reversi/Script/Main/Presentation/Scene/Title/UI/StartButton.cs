using UnityEngine;
using UnityEngine.UI;
using Pommel.Reversi.Presentation.Scene.Title.Dispatcher;
using System;

namespace Pommel.Reversi.Presentation.Scene.Title.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class StartButton : MonoBehaviour, IDisposable
    {
        [SerializeField]
        private Button m_button;

        private IDisposable m_startDisposable;

        public Button Button => m_button;

        public void Initialize()
        {
            m_startDisposable = this.ApplyChangeSceneEvent();
        }

        public void Dispose()
        {
            m_startDisposable.Dispose();
            m_startDisposable = null;
        }
    }
}

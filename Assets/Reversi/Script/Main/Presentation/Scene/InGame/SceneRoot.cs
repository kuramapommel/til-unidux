using Pommel.Reversi.Presentation.Project.SceneChange;
using Pommel.Reversi.Presentation.Scene.InGame.UI;
using UniRx.Async;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Pommel.Reversi.Presentation.Scene.InGame
{
    public sealed class SceneRoot : SceneBase
    {
        protected override Page Page => Page.InGamePage;

        [SerializeField]
        private ResultMessage m_resultMessage;

        [SerializeField]
        private Board m_board;

        private IEnumerable<IDisposable> m_disposables;

        protected override async UniTask OnLoad()
        {
            m_disposables = new[]
            {
                m_resultMessage,
                m_board as IDisposable
            };
        }

        protected override async UniTask OnOpen()
        {
            m_resultMessage.Initialize();
            m_board.Initialize();
        }

        protected override async UniTask OnDispose()
        {
            foreach (var disposable in m_disposables)
            {
                disposable.Dispose();
            }
            m_disposables.ToList().Clear();
            m_disposables = null;
        }
    }
}
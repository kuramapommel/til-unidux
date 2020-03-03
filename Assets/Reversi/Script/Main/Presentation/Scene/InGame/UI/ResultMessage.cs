using System.Threading;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;
using System;
using Pommel.Reversi.Presentation.Scene.InGame.Dispatcher;
using Pommel.Reversi.Presentation.Scene.InGame.Renderer;
using System.Collections.Generic;
using System.Linq;

namespace Pommel.Reversi.Presentation.Scene.InGame.UI
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Animator))]
    public class ResultMessage : MonoBehaviour, IDisposable
    {
        [SerializeField]
        private Button m_button;

        [SerializeField]
        private Text m_text;

        [SerializeField]
        private Animator m_openAnimator;

        public Button Button => m_button;

        private IEnumerable<IDisposable> m_disposables;

        private readonly string OPEN_ANIM_TRIGGER = "OnEnable";

        private readonly string OPEN_ANIM_NAME = "Open";

        public void Initialize()
        {
            m_disposables = new[]
            {
                this.ApplyChangeSceneEvent(),
                this.ApplyOpenResultMessageAnimation()
            };

            transform.localScale = new Vector3(transform.localScale.x, 0f, transform.localScale.z);
            gameObject.SetActive(false);
        }

        public void SetMessageText(string text) => m_text.text = text;

        public async UniTask Open(CancellationTokenSource cancelTokenSource = default)
        {
            gameObject.SetActive(true);
            m_openAnimator.SetTrigger(OPEN_ANIM_TRIGGER);
            await UniTask.WaitUntil(() => m_openAnimator.GetCurrentAnimatorStateInfo(0).IsName(OPEN_ANIM_NAME), cancellationToken: cancelTokenSource.Token);
            await UniTask.WaitWhile(() => m_openAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f, cancellationToken: cancelTokenSource.Token);
            m_openAnimator.enabled = false;
            transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
        }

        public void Dispose()
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

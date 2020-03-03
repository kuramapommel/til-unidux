using System;
using System.Linq;
using System.Threading;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;

namespace Pommel.Reversi.Presentation.Scene.InGame
{
    public sealed class ResultRenderer : MonoBehaviour
    {
        [SerializeField]
        private Text m_resultMessage;

        [SerializeField]
        private Animator m_animator;

        private readonly string OPEN_ANIM_TRIGGER = "OnEnable";

        private readonly string OPEN_ANIM_NAME = "Open";

        public void Initialize()
        {
            _ = Unidux
                .Subject
                .TakeUntilDestroy(this)
                .StartWith(Unidux.State)
                .Where(state => state.Result.Winner != WinnerStateElement.State.Undecide)
                .Subscribe(async state =>
                {
                    var cancelTokenSource = new CancellationTokenSource();
                    string getResultMessage()
                    {
                        switch (state.Result.Winner)
                        {
                            case WinnerStateElement.State.Black: return "Black Win.";
                            case WinnerStateElement.State.White: return "White Win.";
                            case WinnerStateElement.State.Draw: return "Draw Game.";
                        }

                        throw new ArgumentOutOfRangeException();
                    }

                    m_resultMessage.text = getResultMessage();

                    gameObject.SetActive(true);
                    m_animator.SetTrigger(OPEN_ANIM_TRIGGER);
                    await UniTask.WaitUntil(() => m_animator.GetCurrentAnimatorStateInfo(0).IsName(OPEN_ANIM_NAME), cancellationToken: cancelTokenSource.Token);
                    await UniTask.WaitWhile(() => m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f, cancellationToken: cancelTokenSource.Token);
                    m_animator.enabled = false;

                    transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
                })
                .AddTo(this);

            transform.localScale = new Vector3(transform.localScale.x, 0f, transform.localScale.z);
            gameObject.SetActive(false);
        }
    }
}

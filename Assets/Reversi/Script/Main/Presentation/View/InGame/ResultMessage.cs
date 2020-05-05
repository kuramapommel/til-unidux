using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Presentation.Model.InGame;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Pommel.Reversi.Presentation.View.InGame
{
    public interface IResultMessage
    {
    }

    [RequireComponent(typeof(Animator))]
    public sealed class ResultMessage : MonoBehaviour, IResultMessage
    {
        private Animator m_animator;

        private ObservableStateMachineTrigger m_stateMachineTrigger;

        private Text m_text;

        [Inject]
        public void Construct(IGameModel model)
        {
            m_animator = GetComponent<Animator>();
            m_stateMachineTrigger = m_animator.GetBehaviour<ObservableStateMachineTrigger>();
            m_text = GetComponentInChildren<Text>();

            m_stateMachineTrigger
                .OnStateExitAsObservable()
                .TakeUntilDestroy(this)
                .Where(on => on.StateInfo.IsName("Open"))
                .SkipWhile(on => on.StateInfo.normalizedTime <= 1.0f)
                .Subscribe(_ => transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z));

            model.Winner
                .Subscribe(winner =>
                {
                    switch (winner)
                    {
                        case Winner.White: m_text.text = "White Win."; break;
                        case Winner.Black: m_text.text = "Black Win."; break;
                        case Winner.Draw: m_text.text = "Draw Game."; break;
                    }

                    gameObject.SetActive(true);
                    m_animator.SetTrigger("OnEnable");
                });

            transform.localScale = new Vector3(transform.localScale.x, 0f, transform.localScale.z);
            gameObject.SetActive(false);
        }
    }
}
using Cysharp.Threading.Tasks;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Presentation.State.InGame;
using Pommel.Reversi.Presentation.State.System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using _Scene = Pommel.Reversi.Domain.Transition.Scene;

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
        public void Construct(IGameState state, ITransitionState transitionState)
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

            GetComponent<Button>()
                .OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ => state.CreateGameAsync().AsUniTask()
                    .ContinueWith(__ => transitionState.AddAsync(_Scene.Title))
                    .ContinueWith(__ => transitionState.RemoveAsync(_Scene.InGame))
                    .Forget()
                    );

            state.Winner
                .TakeUntilDestroy(this)
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
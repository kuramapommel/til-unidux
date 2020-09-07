using System.Linq;
using Cysharp.Threading.Tasks;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Reducks.InGame;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static Pommel.Reversi.Reducks.InGame.Selectors;

namespace Pommel.Reversi.Components.InGame
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
        public void Construct(
            IOperation operation,
            IStateAsObservableCreator observableCreator
            )
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
                .Subscribe(_ => operation.ReturnToTitle().ToObservable());

            observableCreator.Create(this, state => state)
                .Where(state => GetGameState(state) == ValueObjects.State.Finished)
                .Select(GetStones)
                .Subscribe(stones =>
                {
                    var summary = stones
                        .Aggregate(
                            (light: 0, dark: 0),
                            (aggregate, stone) =>
                            {
                                switch (stone.StoneColor)
                                {
                                    case ValueObjects.Stone.Color.Light: return (++aggregate.light, aggregate.dark);
                                    case ValueObjects.Stone.Color.Dark: return (aggregate.light, ++aggregate.dark);
                                }

                                return aggregate;
                            }
                        );

                    switch (summary)
                    {
                        case var _ when summary.light > summary.dark:
                            {
                                m_text.text = "Light Win.";
                                return;
                            }
                        case var _ when summary.dark > summary.light:
                            {
                                m_text.text = "Dark Win.";
                                return;
                            }
                    }

                    m_text.text = "Draw Game.";
                });

            transform.localScale = new Vector3(transform.localScale.x, 0f, transform.localScale.z);
            gameObject.SetActive(false);
        }
    }
}
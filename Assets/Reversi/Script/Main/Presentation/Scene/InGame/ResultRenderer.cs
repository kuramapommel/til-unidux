using System;
using System.Linq;
using UniRx;
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

        private readonly string OPEN_ANIM = "OnEnable";

        private void Start()
        {
            _ = Unidux
                .Subject
                .TakeUntilDestroy(this)
                .StartWith(Unidux.State)
                .Where(state => state.Result.Winner != WinnerStateElement.State.Undecide)
                .Subscribe(state =>
                {
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
                    m_animator.SetTrigger(OPEN_ANIM);
                })
                .AddTo(this);

            gameObject.SetActive(false);
        }
    }
}

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

        private void Awake()
        {
            gameObject.SetActive(false);

            _ = Unidux
                .Subject
                .TakeUntilDisable(this)
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
                })
                .AddTo(this);
        }
    }
}

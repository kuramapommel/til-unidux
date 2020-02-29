using Pommel.Reversi.Presentation.Project.SceneChange;
using Unidux.SceneTransition;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using ProjectUnidux = Pommel.Reversi.Presentation.Project.Unidux;
using SceneType = Pommel.Reversi.Presentation.Project.SceneChange.Scene;

namespace Pommel.Reversi.Presentation.Scene.InGame
{
    public sealed class ResultDispatcher : MonoBehaviour
    {
        [SerializeField]
        private Button m_button;

        private void OnEnable()
        {
            _ = m_button.OnClickAsObservable()
                .TakeUntilDisable(this)
                .Select(_ => PageDuck<Page, SceneType>.ActionCreator.Push(Page.TitlePage))
                .Subscribe(action => ProjectUnidux.Dispatch(action))
                .AddTo(this);
        }
    }
}
using Pommel.Reversi.Presentation.Project.SceneChange;
using Unidux.SceneTransition;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using ProjectUnidux = Pommel.Reversi.Presentation.Project.Unidux;
using SceneType = Pommel.Reversi.Presentation.Project.SceneChange.Scene;

namespace Pommel.Reversi.Presentation.Scene.Title
{
    public sealed class StartDispatcher : MonoBehaviour
    {
        [SerializeField]
        private Button m_button;

        private void Start()
        {
            _ = m_button.OnClickAsObservable()
                .TakeUntilDisable(this)
                .Select(_ => PageDuck<Page, SceneType>.ActionCreator.Push(Page.InGamePage))
                .Subscribe(action => ProjectUnidux.Dispatch(action))
                .AddTo(this);
        }
    }
}

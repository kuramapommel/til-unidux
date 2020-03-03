using Pommel.Reversi.Presentation.Project;
using Pommel.Reversi.Presentation.Project.SceneChange;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Pommel.Reversi.Presentation.Scene.InGame
{
    public sealed class ResultDispatcher : MonoBehaviour
    {
        [SerializeField]
        private Button m_button;

        public void Initialize()
        {
            _ = m_button.OnClickAsObservable()
                .TakeUntilDisable(this)
                .Subscribe(action => GameCore.ChangeScene(Page.TitlePage))
                .AddTo(this);
        }
    }
}
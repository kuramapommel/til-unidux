using Pommel.Reversi.Presentation.Project;
using Pommel.Reversi.Presentation.Project.SceneChange;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Pommel.Reversi.Presentation.Scene.Title
{
    public sealed class StartDispatcher : MonoBehaviour
    {
        [SerializeField]
        private Button m_button;

        public void Initialize()
        {
            _ = m_button.OnClickAsObservable()
                .Subscribe(action => GameCore.ChangeScene(Page.InGamePage))
                .AddTo(this);
        }
    }
}

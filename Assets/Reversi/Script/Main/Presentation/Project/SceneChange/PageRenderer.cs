using Unidux.SceneTransition;
using UniRx;
using UnityEngine;

namespace Pommel.Reversi.Presentation.Project.SceneChange
{
    public class PageRenderer : MonoBehaviour
    {
        private readonly ISceneConfig<Scene, Page> config = new SceneConfig();

        void Start()
        {
            _ = Unidux.Subject
                    .Where(state => state.Page.IsStateChanged)
                    .StartWith(Unidux.State)
                    .Where(state => state.Page.IsReady && state.Scene.NeedsAdjust(config.GetPageScenes(), config.PageMap[state.Page.Current.Page]))
                    .Subscribe(_ => Unidux.Dispatch(PageDuck<Page, Scene>.ActionCreator.Adjust()))
                    .AddTo(this);
        }
    }
}

using Unidux;
using Unidux.SceneTransition;
using UnityEngine;
using Page = Pommel.Reversi.Presentation.Project.SceneChange.Page;
using SceneType = Pommel.Reversi.Presentation.Project.SceneChange.Scene;

namespace Pommel.Reversi.Presentation.Project
{
    public sealed class SceneRoot : SingletonMonoBehaviour<SceneRoot>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeGame()
        {
            Unidux.Dispatch(PageDuck<Page, SceneType>.ActionCreator.Reset());
            Unidux.Dispatch(PageDuck<Page, SceneType>.ActionCreator.Push(Page.TitlePage));
        }
    }
}

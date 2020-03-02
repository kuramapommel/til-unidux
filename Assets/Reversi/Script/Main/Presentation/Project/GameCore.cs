using Pommel.Reversi.Presentation.Project.SceneChange;
using Unidux;
using Unidux.SceneTransition;
using UniRx.Async;
using UnityEngine;
using Page = Pommel.Reversi.Presentation.Project.SceneChange.Page;
using SceneType = Pommel.Reversi.Presentation.Project.SceneChange.Scene;

namespace Pommel.Reversi.Presentation.Project
{
    public sealed class GameCore : SingletonMonoBehaviour<GameCore>
    {
        private const string OBJECT_NAME = "GameCore";

        [SerializeField]
        private PageRenderer m_pageRenderer;

        [SerializeField]
        private SceneRenderer m_sceneRenderer;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static async UniTask InitializeGame()
        {
            var core = await Resources.LoadAsync<GameObject>(OBJECT_NAME);
            var instance = Instantiate(core);
            instance.name = OBJECT_NAME;
        }

        protected override void Awake()
        {
            if (!CheckInstance()) return;

            Unidux.Dispatch(PageDuck<Page, SceneType>.ActionCreator.Reset());
            Unidux.Dispatch(PageDuck<Page, SceneType>.ActionCreator.Push(Page.TitlePage));

            DontDestroyOnLoad(gameObject);

            var pageRenderer = Instantiate(m_pageRenderer);
            pageRenderer.transform.SetParent(transform);
            pageRenderer.Init();

            var sceneRenderer = Instantiate(m_sceneRenderer);
            sceneRenderer.transform.SetParent(transform);
            sceneRenderer.Init();
        }
    }
}

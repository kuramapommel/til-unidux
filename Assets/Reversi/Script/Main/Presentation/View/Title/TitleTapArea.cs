using Pommel.Reversi.Presentation.Model.InGame;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Pommel.Reversi.Presentation.View.Title
{
    public interface ITitleTapArea
    {
    }

    [RequireComponent(typeof(Button))]
    public sealed class TitleTapArea : MonoBehaviour, ITitleTapArea
    {
        private Button m_tapArea;

        private ZenjectSceneLoader m_sceneLoader;

        [Inject]
        public void Construct(IGameModel model, ZenjectSceneLoader sceneLoader)
        {
            m_sceneLoader = sceneLoader;
            m_tapArea = GetComponent<Button>();
            m_tapArea
                .OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(async _ =>
                {
                    await model.CreateGameAsync();
                    await m_sceneLoader.LoadSceneAsync(
                        "InGame",
                        LoadSceneMode.Additive,
                        container => container.Bind<IGameModel>().FromInstance(model).AsCached());
                    await SceneManager.UnloadSceneAsync("Title");
                    await model.Start();
                });
        }
    }
}

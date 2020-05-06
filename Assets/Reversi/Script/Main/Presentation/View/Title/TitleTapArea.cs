using Pommel.Reversi.Presentation.Model.InGame;
using Pommel.Reversi.Presentation.Model.System;
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

        [Inject]
        public void Construct(IGameModel model, ITransitionModel transitionModel)
        {
            m_tapArea = GetComponent<Button>();
            m_tapArea
                .OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ =>
                    model.CreateGameAsync()
                    .ContinueWith(__ => transitionModel.LoadSceneAsync(
                        loadSceneName: "InGame",
                        mode: LoadSceneMode.Additive,
                        unloadSceneName: "Title",
                        container => container.Bind<IGameModel>().FromInstance(model).AsCached()))
                    .ContinueWith(() => model.Start())
                    .ToObservable()
                    );
        }
    }
}

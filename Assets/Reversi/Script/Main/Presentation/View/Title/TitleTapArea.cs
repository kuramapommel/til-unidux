using Pommel.Reversi.Presentation.State.InGame;
using Pommel.Reversi.Presentation.State.System;
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
        public void Construct(IGameState state, ITransitionState transitionState)
        {
            m_tapArea = GetComponent<Button>();
            m_tapArea
                .OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ =>
                    state.CreateGameAsync().AsUniTask()
                    .ContinueWith(__ => transitionState.LoadSceneAsync(
                        loadSceneName: "InGame",
                        mode: LoadSceneMode.Additive,
                        unloadSceneName: "Title",
                        container => container.Bind<IGameState>().FromInstance(state).AsCached()))
                    .ContinueWith(() => state.Start().AsUniTask())
                    .ToObservable()
                    );
        }
    }
}

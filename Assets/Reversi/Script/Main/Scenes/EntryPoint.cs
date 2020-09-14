using Unidux.SceneTransition;
using UniRx;
using UnityEngine;
using Zenject;
using static Pommel.Reversi.Domain.Scene.ValueObjects;
using static Pommel.Reversi.Reducks.EntryPoint.Operations;
using static Pommel.Reversi.Reducks.Transition.Operations;

namespace Pommel.Reversi.Scenes
{
    public interface IEntryPoint
    {
    }

    public sealed class EntryPoint : MonoBehaviour, IEntryPoint
    {
        [Inject]
        public void Construct(
            IStateAsObservableCreator observableCreator,
            IDispatcher dispatcher,
            ISceneConfig<Scene, Page> config,
            ILoadableScene loadableScene,
            ILoadableTitle loadableTitle,
            IAdjustablePages adjustablePages
            )
        {
            observableCreator.Create(this, state => state.Scene.IsStateChanged, state => state.Scene)
                .Subscribe(scene => dispatcher.Dispatch(loadableScene.LoadScene(scene)));

            observableCreator.Create(this, state => state.Page.IsStateChanged, state => state)
                .Where(state => state.Page.IsReady
                    && state.Scene.NeedsAdjust(config.GetPageScenes(), config.PageMap[state.Page.Current.Page]))
                .Subscribe(scene => dispatcher.Dispatch(adjustablePages.AdjustPages()));

            dispatcher.Dispatch(loadableTitle.LoadTitle());
        }
    }
}
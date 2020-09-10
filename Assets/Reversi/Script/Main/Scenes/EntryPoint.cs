using Unidux.SceneTransition;
using UniRx;
using UnityEngine;
using Zenject;
using static Pommel.Reversi.Domain.Scene.ValueObjects;
using IEntryPointOperation = Pommel.Reversi.Reducks.EntryPoint.IOperation;
using ITransitionOperation = Pommel.Reversi.Reducks.Transition.IOperation;

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
            ISceneConfig<Scene, Page> config,
            ITransitionOperation transitionOperation,
            IEntryPointOperation entryPointOperation
            )
        {
            observableCreator.Create(this, state => state.Scene.IsStateChanged, state => state.Scene)
                .Subscribe(scene => transitionOperation.LoadScene(scene));

            observableCreator.Create(this, state => state.Page.IsStateChanged, state => state)
                .Where(state => state.Page.IsReady
                    && state.Scene.NeedsAdjust(config.GetPageScenes(), config.PageMap[state.Page.Current.Page]))
                .Subscribe(scene => transitionOperation.AdjustPages());

            _ = entryPointOperation.ToTitle();
        }
    }
}
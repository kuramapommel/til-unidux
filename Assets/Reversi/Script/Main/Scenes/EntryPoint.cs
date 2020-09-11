using Unidux.SceneTransition;
using UniRx;
using UnityEngine;
using Zenject;
using static Pommel.Reversi.Domain.Scene.ValueObjects;
using IEntryPointOperationFactory = Pommel.Reversi.Reducks.EntryPoint.Operation.IFactory;
using ITransitionOperationFactory = Pommel.Reversi.Reducks.Transition.Operation.IFactory;

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
            IEntryPointOperationFactory entryPointOperationFactory,
            ITransitionOperationFactory transitionOperationFactory
            )
        {
            var entryPointOperation = entryPointOperationFactory.Create();
            var transitionOperation = transitionOperationFactory.Create();

            observableCreator.Create(this, state => state.Scene.IsStateChanged, state => state.Scene)
                .Subscribe(scene => dispatcher.Dispatch(transitionOperation.LoadScene(scene)));

            observableCreator.Create(this, state => state.Page.IsStateChanged, state => state)
                .Where(state => state.Page.IsReady
                    && state.Scene.NeedsAdjust(config.GetPageScenes(), config.PageMap[state.Page.Current.Page]))
                .Subscribe(scene => dispatcher.Dispatch(transitionOperation.AdjustPages()));

            dispatcher.Dispatch(entryPointOperation.ToTitle());
        }
    }
}
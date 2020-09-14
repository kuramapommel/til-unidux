using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Pommel.Reversi.Domain.Scene;
using Unidux.SceneTransition;

namespace Pommel.Reversi.Reducks.Transition
{
    public static class Operations
    {
        public interface IAdjustablePages
        {
            Func<Func<IDispatcher, Task>> AdjustPages { get; }
        }

        public interface ILoadableScene
        {
            Func<SceneState<ValueObjects.Scene>, Func<IDispatcher, Task>> LoadScene { get; }
        }
    }

    public static class OperationImpls
    {
        public sealed class AdjustPagesOperation : Operations.IAdjustablePages
        {
            public Func<Func<IDispatcher, Task>> AdjustPages { get; } = () => async dispatcher => dispatcher.Dispatch(PageDuck<ValueObjects.Page, ValueObjects.Scene>.ActionCreator.Adjust());
        }

        public sealed class LoadSceneOperation : Operations.ILoadableScene
        {
            public Func<SceneState<ValueObjects.Scene>, Func<IDispatcher, Task>> LoadScene { get; } = sceneState => async dispatcher =>
            {
                await UniTask.WhenAll(
                    sceneState.Removals(SceneUtil.GetActiveScenes<ValueObjects.Scene>())
                    .Select(scene => SceneUtil.Remove(scene.ToString()).ToUniTask())
                    );

                await UniTask.WhenAll(
                    sceneState.Additionals(SceneUtil.GetActiveScenes<ValueObjects.Scene>())
                    .Select(scene => SceneUtil.Add(scene.ToString()).ToUniTask())
                    );
            };
        }
    }
}
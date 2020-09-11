using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Pommel.Reversi.Domain.Scene;
using Unidux.SceneTransition;

namespace Pommel.Reversi.Reducks.Transition
{
    public interface IOperation
    {
        Func<Func<IDispatcher, Task>> AdjustPages { get; }

        Func<SceneState<ValueObjects.Scene>, Func<IDispatcher, Task>> LoadScene { get; }
    }

    public static class Operation
    {
        public interface IFactory
        {
            IOperation Create();
        }

        public sealed class Impl : IOperation
        {
            public Func<Func<IDispatcher, Task>> AdjustPages { get; }

            public Func<SceneState<ValueObjects.Scene>, Func<IDispatcher, Task>> LoadScene { get; }

            public Impl(
            )
            {
                AdjustPages = () => async dispatcher => dispatcher.Dispatch(PageDuck<ValueObjects.Page, ValueObjects.Scene>.ActionCreator.Adjust());

                LoadScene = sceneState => async dispatcher =>
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
}
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
        Func<Task> AdjustPages { get; }

        Func<SceneState<ValueObjects.Scene>, Task> LoadScene { get; }
    }

    public sealed class Operation : IOperation
    {
        public Func<Task> AdjustPages { get; }

        public Func<SceneState<ValueObjects.Scene>, Task> LoadScene { get; }

        public Operation(
            IDispatcher dispatcher
        )
        {
            AdjustPages = async () => dispatcher.Dispatch(PageDuck<ValueObjects.Page, ValueObjects.Scene>.ActionCreator.Adjust());

            LoadScene = async sceneState =>
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
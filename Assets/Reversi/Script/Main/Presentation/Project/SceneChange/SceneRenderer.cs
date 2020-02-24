using Unidux.SceneTransition;
using UniRx;
using UniRx.Async;
using UnityEngine;

namespace Pommel.Reversi.Presentation.Project.SceneChange
{
    public sealed class SceneRenderer : MonoBehaviour
    {
        void Start()
        {
            _ = Unidux
                .Subject
                .Where(state => state.Scene.IsStateChanged)
                .StartWith(Unidux.State)
                .Subscribe(async state =>
                {
                    await UniTask.WhenAll(state.Scene.Removals(SceneUtil.GetActiveScenes<Scene>())
                        .Select(scene => SceneUtil.Remove(scene.ToString()).ToUniTask()));

                    await UniTask.WhenAll(state.Scene.Additionals(SceneUtil.GetActiveScenes<Scene>())
                        .Select(scene => SceneUtil.Add(scene.ToString()).ToUniTask()));
                })
                .AddTo(this);
        }
    }
}

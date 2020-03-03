using System;
using Pommel.Reversi.Presentation.Project.SceneChange;
using UniRx.Async;
using UnityEngine;

namespace Pommel.Reversi.Presentation.Scene
{
    public abstract class SceneBase : MonoBehaviour
    {
        private async UniTask Awake()
        {
            try
            {
                await OnLoad();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(
                    "Initialize error," +
                    $"scene {Page}." +
                    $"trace: {e}"
                    );
            }
        }

        private async UniTask Start()
        {
            try
            {
                await OnOpen();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(
                    "open error," +
                    $"scene {Page}." +
                    $"trace: {e}"
                    );
            }
        }

        private async UniTask OnDestroy()
        {
            try
            {
                await OnDispose();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(
                    "dispose error," +
                    $"scene {Page}." +
                    $"trace: {e}"
                    );
            }
        }

        protected abstract Page Page { get; }

        protected abstract UniTask OnLoad();

        protected abstract UniTask OnOpen();

        protected abstract UniTask OnDispose();
    }
}

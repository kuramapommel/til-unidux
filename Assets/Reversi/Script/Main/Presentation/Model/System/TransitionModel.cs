using System;
using UniRx.Async;
using UnityEngine.SceneManagement;
using Zenject;

namespace Pommel.Reversi.Presentation.Model.System
{
    public interface ITransitionModel
    {
        UniTask LoadSceneAsync(string loadSceneName, LoadSceneMode mode = LoadSceneMode.Single, string unloadSceneName = default, Action<DiContainer> bind = default);
    }

    public sealed class TransitionModel : ITransitionModel
    {
        private readonly ZenjectSceneLoader m_sceneLoader;

        public TransitionModel(ZenjectSceneLoader sceneLoader)
        {
            m_sceneLoader = sceneLoader;
        }

        public async UniTask LoadSceneAsync(string loadSceneName, LoadSceneMode mode = LoadSceneMode.Single, string unloadSceneName = default, Action<DiContainer> bind = default)
        {
            await m_sceneLoader.LoadSceneAsync(loadSceneName, mode, bind ?? (_ => { }));
            if (unloadSceneName != default) await SceneManager.UnloadSceneAsync(unloadSceneName);
        }
    }
}
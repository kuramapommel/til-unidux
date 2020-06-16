using System;
using System.Threading.Tasks;
using UniRx.Async;
using UnityEngine.SceneManagement;
using Zenject;

namespace Pommel.Reversi.Presentation.Model.System
{
    public interface ITransitionModel
    {
        Task LoadSceneAsync(string loadSceneName, Action<DiContainer> bind = default);

        Task AddSceneAsync(string loadSceneName, Action<DiContainer> bind = default);

        Task RemoveSceneAsync(string unloadSceneName);
    }

    public sealed class TransitionModel : ITransitionModel
    {
        private readonly ZenjectSceneLoader m_sceneLoader;

        public async Task LoadSceneAsync(string loadSceneName, Action<DiContainer> bind = default) =>
            await m_sceneLoader.LoadSceneAsync(loadSceneName, LoadSceneMode.Single, bind ?? (_ => { }));

        public async Task AddSceneAsync(string loadSceneName, Action<DiContainer> bind = default) =>
            await m_sceneLoader.LoadSceneAsync(loadSceneName, LoadSceneMode.Additive, bind ?? (_ => { }));

        public async Task RemoveSceneAsync(string unloadSceneName) =>
            await SceneManager.UnloadSceneAsync(unloadSceneName);
    }
}
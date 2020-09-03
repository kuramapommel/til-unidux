using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Pommel.Reversi.Domain.Transition;
using UnityEngine.SceneManagement;
using Zenject;

namespace Pommel.Reversi.Presentation.Model.System
{
    public interface ITransitionModel : IDisposable
    {
        Task LoadSceneAsync(IScene scene, Action<DiContainer> bind = default);

        Task UnloadSceneAsync(IScene scene);
    }

    public sealed class TransitionModel : ITransitionModel
    {
        private readonly ZenjectSceneLoader m_sceneLoader;

        public TransitionModel(ZenjectSceneLoader sceneLoader) => m_sceneLoader = sceneLoader;

        public async Task LoadSceneAsync(IScene scene, Action<DiContainer> bind = default) =>
            await m_sceneLoader.LoadSceneAsync(scene.Id, scene.IsBase ? LoadSceneMode.Single : LoadSceneMode.Additive, bind ?? (_ => { }));

        public async Task UnloadSceneAsync(IScene scene) =>
            await SceneManager.UnloadSceneAsync(scene.Id);

        void IDisposable.Dispose()
        {
        }
    }
}
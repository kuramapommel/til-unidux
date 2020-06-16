using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pommel.Reversi.Domain.Transition;
using Pommel.Reversi.Presentation.Model.System;
using UniRx;
using UniRx.Async;
using UnityEngine.SceneManagement;
using Zenject;

namespace Pommel.Reversi.Presentation.State.System
{
    public interface ITransitionState
    {
        UniTask LoadSceneAsync(string loadSceneName, LoadSceneMode mode = LoadSceneMode.Single, string unloadSceneName = default, Action<DiContainer> bind = default);
    }

    public sealed class TransitionState : ITransitionState
    {
        private readonly ZenjectSceneLoader m_sceneLoader;

        private readonly ITransitionModel m_transitionModel;

        // todo tuple やめて scene entity を domain に切る
        private readonly IReactiveCollection<IScene> m_scenes = new ReactiveCollection<IScene>();

        public TransitionState(ZenjectSceneLoader sceneLoader)
        {
            m_sceneLoader = sceneLoader;

            m_scenes
                .ObserveAdd()
                .Subscribe(addEvent => m_transitionModel.LoadSceneAsync(addEvent.Value).AsUniTask().Forget());

            m_scenes
                .ObserveRemove()
                .Subscribe(removeEvent => m_transitionModel.UnloadSceneAsync(removeEvent.Value).AsUniTask().Forget());
        }

        public async UniTask LoadSceneAsync(string loadSceneName, LoadSceneMode mode = LoadSceneMode.Single, string unloadSceneName = default, Action<DiContainer> bind = default)
        {
            await m_sceneLoader.LoadSceneAsync(loadSceneName, mode, bind ?? (_ => { }));
            if (unloadSceneName != default) await SceneManager.UnloadSceneAsync(unloadSceneName);
        }

        public async Task LoadAsync(IScene baseScene, params IScene[] childrenScenes) => await LoadAsync(baseScene, childrenScenes);

        public async Task LoadAsync(IScene baseScene, IEnumerable<IScene> childrenScenes)
        {
            await UniTask.CompletedTask;

            m_scenes.Clear();
            m_scenes.Add(baseScene);
            foreach (var scene in childrenScenes)
            {
                m_scenes.Add(scene);
            }
        }

        public async Task AddAsync(params IScene[] scenes) => await AddAsync(scenes.AsEnumerable());

        public async Task AddAsync(IEnumerable<IScene> scenes)
        {
            await UniTask.CompletedTask;

            foreach (var scene in scenes)
            {
                m_scenes.Add(scene);
            }
        }

        public async Task RemoveAsync(params IScene[] scenes) => await RemoveAsync(scenes.AsEnumerable());

        public async Task RemoveAsync(IEnumerable<IScene> scenes)
        {
            await UniTask.CompletedTask;

            foreach (var scene in scenes)
            {
                m_scenes.Remove(scene);
            }
        }
    }
}
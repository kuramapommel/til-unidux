using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IReactiveCollection<(string name, bool isBase)> m_scenes = new ReactiveCollection<(string name, bool isBase)>();

        public TransitionState(ZenjectSceneLoader sceneLoader)
        {
            m_sceneLoader = sceneLoader;

            m_scenes
                .ObserveAdd()
                .Subscribe(addEvent => (
                    addEvent.Value.isBase
                        ? m_transitionModel.LoadSceneAsync(addEvent.Value.name).AsUniTask()
                        : m_transitionModel.AddSceneAsync(addEvent.Value.name).AsUniTask()
                    ).Forget());

            m_scenes
                .ObserveRemove()
                .Subscribe(removeEvent => m_transitionModel.RemoveSceneAsync(removeEvent.Value.name).AsUniTask().Forget());
        }

        public async UniTask LoadSceneAsync(string loadSceneName, LoadSceneMode mode = LoadSceneMode.Single, string unloadSceneName = default, Action<DiContainer> bind = default)
        {
            await m_sceneLoader.LoadSceneAsync(loadSceneName, mode, bind ?? (_ => { }));
            if (unloadSceneName != default) await SceneManager.UnloadSceneAsync(unloadSceneName);
        }

        public async Task LoadAsync(string baseSceneName, params string[] childrenSceneNames) => await LoadAsync(baseSceneName, childrenSceneNames);

        public async Task LoadAsync(string baseSceneName, IEnumerable<string> childrenSceneNames)
        {
            await UniTask.CompletedTask;

            m_scenes.Clear();
            m_scenes.Add((baseSceneName, true));
            foreach (var name in childrenSceneNames)
            {
                m_scenes.Add((name, false));
            }
        }

        public async Task AddAsync(params string[] sceneNames) => await AddAsync(sceneNames.AsEnumerable());

        public async Task AddAsync(IEnumerable<string> sceneNames)
        {
            await UniTask.CompletedTask;

            foreach (var name in sceneNames)
            {
                m_scenes.Add((name, false));
            }
        }

        public async Task RemoveAsync(params string[] sceneNames) => await RemoveAsync(sceneNames.AsEnumerable());

        public async Task RemoveAsync(IEnumerable<string> sceneNames)
        {
            await UniTask.CompletedTask;

            foreach (var name in sceneNames)
            {
                m_scenes.Remove((name, false));
            }
        }
    }
}
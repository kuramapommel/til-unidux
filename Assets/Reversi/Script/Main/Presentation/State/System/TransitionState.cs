using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pommel.Reversi.Domain.Transition;
using Pommel.Reversi.Presentation.Model.System;
using UniRx;
using UniRx.Async;
using Zenject;

namespace Pommel.Reversi.Presentation.State.System
{
    public interface ITransitionState
    {
        Task AddAsync(IScene scene, Action<DiContainer> bind = default);

        Task RemoveAsync(params IScene[] scenes);

        Task RemoveAsync(IEnumerable<IScene> scenes);
    }

    public sealed class TransitionState : ITransitionState
    {
        private readonly ITransitionModel m_transitionModel;

        private readonly IReactiveDictionary<IScene, Action<DiContainer>> m_transitionMap = new ReactiveDictionary<IScene, Action<DiContainer>>();

        public TransitionState(ITransitionModel transitionModel)
        {
            m_transitionModel = transitionModel;

            m_transitionMap
                .ObserveAdd()
                .Subscribe(addEvent => m_transitionModel.LoadSceneAsync(addEvent.Key, addEvent.Value).AsUniTask().Forget());

            m_transitionMap
                .ObserveRemove()
                .Subscribe(removeEvent =>
                {
                    m_transitionModel.UnloadSceneAsync(removeEvent.Key).AsUniTask().Forget();
                });
        }

        public async Task AddAsync(IScene scene, Action<DiContainer> bind = default)
        {
            await UniTask.CompletedTask;
            m_transitionMap.Add(scene, bind);
        }

        public async Task RemoveAsync(params IScene[] scenes) => await RemoveAsync(scenes.AsEnumerable());

        public async Task RemoveAsync(IEnumerable<IScene> scenes)
        {
            await UniTask.CompletedTask;

            foreach (var scene in scenes)
            {
                m_transitionMap.Remove(scene);
            }
        }
    }
}
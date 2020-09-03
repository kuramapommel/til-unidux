using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Pommel.Reversi.Domain.Transition;
using Pommel.Reversi.Presentation.Model.System;
using Zenject;

namespace Pommel.Reversi.Presentation.ViewModel.System
{
    public interface ITransitionState : IDisposable
    {
        Task LoadAsync(IScene scene, Action<DiContainer> bind = default);

        Task RemoveAsync(params IScene[] scenes);

        Task RemoveAsync(IEnumerable<IScene> scenes);
    }

    public sealed class TransitionState : ITransitionState
    {
        private readonly ITransitionModel m_transitionModel;

        public TransitionState(ITransitionModel transitionModel)
        {
            m_transitionModel = transitionModel;
        }

        public async Task LoadAsync(IScene scene, Action<DiContainer> bind = default) => await m_transitionModel.LoadSceneAsync(scene, bind).AsUniTask();

        public async Task RemoveAsync(params IScene[] scenes) => await RemoveAsync(scenes.AsEnumerable());

        public async Task RemoveAsync(IEnumerable<IScene> scenes) =>
            await scenes.ToUniTaskAsyncEnumerable().ForEachAwaitAsync(async scene => await m_transitionModel.UnloadSceneAsync(scene).AsUniTask());

        void IDisposable.Dispose()
        {
            m_transitionModel.Dispose();
        }
    }
}
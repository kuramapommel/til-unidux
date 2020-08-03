using Pommel.Reversi.Presentation.ViewModel.System;
using UnityEngine;
using Zenject;
using _Scene = Pommel.Reversi.Domain.Transition.Scene;

namespace Pommel.Reversi.Presentation.View.Base
{
    public interface IEntryPoint
    {
        
    }

    public sealed class EntryPoint : MonoBehaviour, IEntryPoint
    {
        [Inject]
        public void Construct(ITransitionState transitionState)
        {
            _ = transitionState.LoadAsync(_Scene.Title);
        }
    }
}
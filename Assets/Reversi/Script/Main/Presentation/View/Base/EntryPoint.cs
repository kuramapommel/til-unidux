using Pommel.Reversi.Presentation.State.System;
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
            _ = transitionState.AddAsync(_Scene.Title);
        }
    }
}
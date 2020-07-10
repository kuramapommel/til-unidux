using System.Linq;
using Cysharp.Threading.Tasks;
using Pommel.Reversi.Presentation.ViewModel.InGame;
using Pommel.Reversi.Presentation.ViewModel.System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using _Scene = Pommel.Reversi.Domain.Transition.Scene;

namespace Pommel.Reversi.Presentation.View.Title
{
    public interface ITitleTapArea
    {
    }

    [RequireComponent(typeof(Button))]
    public sealed class TitleTapArea : MonoBehaviour, ITitleTapArea
    {
        private Button m_tapArea;

        [Inject]
        public void Construct(IGameViewModel state, ITransitionState transitionState)
        {
            // todo どこかで入力するようにする
            var playerId = System.Guid.NewGuid().ToString();
            var name = playerId.Substring(0, 3);

            m_tapArea = GetComponent<Button>();
            m_tapArea
                .OnClickAsObservable()
                .TakeUntilDestroy(this)
                .SelectMany(_ => state.CreateMatchingAsync(playerId, $"player-{name}").AsUniTask().ToObservable())
                .SelectMany(_ => transitionState.AddAsync(_Scene.InGame, container => container.Bind<IGameViewModel>().FromInstance(state).AsCached()).AsUniTask().ToObservable())
                .Subscribe(_ => transitionState.RemoveAsync(_Scene.Title).AsUniTask().Forget());
        }
    }
}

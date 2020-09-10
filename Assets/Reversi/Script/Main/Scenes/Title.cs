using Pommel.Reversi.Components.Title;
using UniRx;
using UnityEngine;
using Zenject;
using static Pommel.Reversi.Reducks.Title.Selectors;

namespace Pommel.Reversi.Scenes
{
    public interface ITile
    {
    }

    public sealed class Title : MonoBehaviour, ITile
    {
        [Inject]
        public void Construct(
            IStateAsObservableCreator observableCreator,
            IGameStartModal gameStartModal)
        {
            observableCreator.Create(this, state => state.Title.IsStateChanged, GetDisplayGameModal)
                .Subscribe(gameStartModal.SetActive);
        }
    }
}
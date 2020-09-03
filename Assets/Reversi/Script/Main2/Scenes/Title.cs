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

    public sealed class Title : MonoBehaviour
    {
        [Inject]
        public void Construct(
            IStateAsObservableCreator observableCreator,
            IGameStartModal gameStartModal)
        {
            observableCreator.Create(this, GetDisplayGameModal)
                .Subscribe(gameStartModal.SetActive);
        }
    }
}
using Pommel.Reversi.Reducks.Title;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Pommel.Reversi.Components.Title
{
    public interface ITitleTapArea
    {
    }

    [RequireComponent(typeof(Button))]
    public sealed class TitleTapArea : MonoBehaviour, ITitleTapArea
    {
        private Button m_tapArea;

        [Inject]
        public void Construct(
            IDispatcher dispatcher,
            Operation.IFactory factory
            )
        {
            m_tapArea = GetComponent<Button>();

            var operation = factory.Create();

            m_tapArea
                .OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ => dispatcher.Dispatch(operation.OpenGameStartModal()));
        }
    }
}

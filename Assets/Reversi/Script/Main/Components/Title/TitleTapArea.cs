using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static Pommel.Reversi.Reducks.Title.Operations;

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
            IOpenableGameStartModal openableGameStartModal
            )
        {
            m_tapArea = GetComponent<Button>();

            m_tapArea
                .OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ => dispatcher.Dispatch(openableGameStartModal.OpenGameStartModal()));
        }
    }
}

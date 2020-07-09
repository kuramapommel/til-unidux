using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Pommel.Reversi.Presentation.ViewModel.Title;

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
        public void Construct(ITitleViewModel titleState)
        {
            m_tapArea = GetComponent<Button>();
            m_tapArea
                .OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ => titleState.TapTitle());
        }
    }
}

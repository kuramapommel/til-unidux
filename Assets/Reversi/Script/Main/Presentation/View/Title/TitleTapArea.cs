using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Pommel.Reversi.Presentation.View.Title
{
    public interface ITitleTapArea
    {
        IObservable<Unit> OnTapAsObservable();
    }

    [RequireComponent(typeof(Button))]
    public sealed class TitleTapArea : MonoBehaviour, ITitleTapArea
    {
        private Button m_tapArea;
        
        public IObservable<Unit> OnTapAsObservable() => m_tapArea.OnClickAsObservable().TakeUntilDestroy(this);

        [Inject]
        public void Construct()
        {
            m_tapArea = GetComponent<Button>();
        }
    }
}

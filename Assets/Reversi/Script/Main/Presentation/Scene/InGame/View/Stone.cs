using System;
using Pommel.Reversi.Presentation.Scene.InGame.State;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Pommel.Reversi.Presentation.Scene.InGame.View
{
    public interface IStone
    {
        IObservable<Unit> OnPutAsObservable();
    }

    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    public sealed class Stone : MonoBehaviour, IStone
    {
        private Button m_button;

        private Image m_image;

        public IObservable<Unit> OnPutAsObservable() => m_button.OnClickAsObservable().TakeUntilDestroy(this);

        [Inject]
        public void Construct(IStoneState stoneState)
        {
            m_button = GetComponent<Button>();
            m_image = GetComponent<Image>();

            stoneState.Color.Subscribe(color => m_image.color = color);
        }
    }
}
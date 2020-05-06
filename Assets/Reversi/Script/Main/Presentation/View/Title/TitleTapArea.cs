using Pommel.Reversi.Presentation.Model.InGame;
using Pommel.Reversi.Presentation.Project; // todo static に依存するのをやめる
using Pommel.Reversi.Presentation.Project.SceneChange;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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
        public void Construct(IGameModel model)
        {
            m_tapArea = GetComponent<Button>();
            m_tapArea
                .OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(async _ =>
                {
                    await model.CreateGameAsync();
                    GameCore.ChangeScene(Page.InGamePage);
                });
        }
    }
}

using System.Linq;
using Pommel.Reversi.Presentation.ViewModel.InGame;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Pommel.Reversi.Presentation.View.InGame
{
    public interface IPlayerInfo
    {
    }

    public interface IFirstPlayerInfo : IPlayerInfo
    {
    }

    public interface ISecondPlayerInfo : IPlayerInfo
    {
    }

    [RequireComponent(typeof(RectTransform))]
    public sealed class PlayerInfo : MonoBehaviour, IFirstPlayerInfo, ISecondPlayerInfo
    {
        [SerializeField]
        private bool m_isFirst = false;

        private Text m_nameText;

        private Text m_colorText;

        [Inject]
        public void Construct(IGameViewModel gameState)
        {
            (m_nameText, m_colorText) = GetComponentsInChildren<Text>()
                .Aggregate(
                    (name: default(Text), color: default(Text)),
                    (playerInfo, text) =>
                    {
                        switch (text.name)
                        {
                            case "Name": return (text, playerInfo.color);
                            case "Color": return (playerInfo.name, text);
                        }

                        return playerInfo;
                    }
                );

            var onInitializePlayer = m_isFirst
                ? gameState.OnInitializeFirstPlayer()
                : gameState.OnInitializeSecondPlayer();

            onInitializePlayer
                .Subscribe(playerState =>
                {
                    playerState.IsTurnPlayer
                        .TakeUntilDestroy(this)
                        .Subscribe(m_colorText.gameObject.SetActive);

                    m_nameText.text = playerState.Name;
                },
                UnityEngine.Debug.Log);

            m_colorText.gameObject.SetActive(m_isFirst);
        }
    }
}
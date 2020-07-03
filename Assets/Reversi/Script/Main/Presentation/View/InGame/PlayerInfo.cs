using System.Linq;
using Pommel.Reversi.Presentation.State.InGame;
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

        private string m_id;

        [Inject]
        public void Construct(IGameState state)
        {
            (m_nameText, m_colorText) = GetComponentsInChildren<Text>()
                .Aggregate(
                    (name: default(Text), color: default(Text)),
                    (playerInfo, text) =>
                    {
                        if (text.name == "Name") return (text, playerInfo.color);
                        if (text.name == "Color") return (playerInfo.name, text);

                        return playerInfo;
                    }
                );

            state.OnStart
                .TakeUntilDestroy(this)
                .Subscribe(game =>
                {
                    var player = m_isFirst
                        ? game.FirstPlayer
                        : game.SecondPlayer;

                    m_id = player.Id;
                    m_nameText.text = player.Name;
                    m_colorText.gameObject.SetActive(m_isFirst);
                });

            state.OnPlayerChanged
                .TakeUntilDestroy(this)
                .Subscribe(playerInfo => m_colorText.gameObject.SetActive(m_id == playerInfo.Id));
        }
    }
}
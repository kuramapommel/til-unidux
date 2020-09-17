using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static Pommel.Reversi.Reducks.InGame.Selectors;

namespace Pommel.Reversi.Components.InGame
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
        public void Construct(
            IStateAsObservableCreator observableCreator
            )
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

            observableCreator.Create(this, state => state.InGame.IsStateChanged, state => GetPlayer(state, m_isFirst))
                .Subscribe(player =>
                {
                    m_colorText.gameObject.SetActive(player.IsTurnPlayer);
                    m_nameText.text = player.Name;
                });

            m_colorText.gameObject.SetActive(m_isFirst);
        }
    }
}
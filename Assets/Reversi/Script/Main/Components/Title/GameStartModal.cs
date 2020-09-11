using System.Linq;
using Pommel.Reversi.Reducks.Title;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Pommel.Reversi.Components.Title
{
    public interface IGameStartModal
    {
        void SetActive(bool isActive);
    }

    public sealed class GameStartModal : MonoBehaviour, IGameStartModal
    {
        private Text m_playerIdText = default;

        private Text m_playerNameText = default;

        private Text m_roomIdText = default;

        private Button m_createRoomButton = default;

        private Button m_entryRoomButton = default;

        [Inject]
        public void Construct(
            IDispatcher dispatcher,
            Operation.IFactory factory
            )
        {
            (m_playerIdText, m_playerNameText, m_roomIdText) = GetComponentsInChildren<InputField>()
                .Aggregate(
                (playerId: default(Text), playerName: default(Text), roomId: default(Text)),
                (aggregate, inputField) =>
                {
                    switch (inputField.name)
                    {
                        case "PlayerIdInputField": return (inputField.textComponent, aggregate.playerName, aggregate.roomId);
                        case "PlayerNameInputField": return (aggregate.playerId, inputField.textComponent, aggregate.roomId);
                        case "RoomIdInputField": return (aggregate.playerId, aggregate.playerName, inputField.textComponent);
                    }

                    return aggregate;
                });

            (m_createRoomButton, m_entryRoomButton) = GetComponentsInChildren<Button>()
                .Aggregate(
                (createRoomButton: default(Button), entryRoomButton: default(Button)),
                (buttons, button) =>
                {
                    switch (button.name)
                    {
                        case "CreateRoomButton": return (button, buttons.entryRoomButton);
                        case "EntryRoomButton": return (buttons.createRoomButton, button);
                    }

                    return buttons;
                });

            var operation = factory.Create();

            m_createRoomButton
                .OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ => dispatcher.Dispatch(operation.CreateRoom(m_playerIdText.text, m_playerNameText.text)));

            m_entryRoomButton
                .OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ => dispatcher.Dispatch(operation.EntryRoom(m_playerIdText.text, m_playerNameText.text, m_roomIdText.text)));
        }

        public void SetActive(bool isActive) => gameObject.SetActive(isActive);
    }
}
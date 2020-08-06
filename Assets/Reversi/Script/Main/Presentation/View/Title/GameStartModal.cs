using System.Linq;
using Cysharp.Threading.Tasks;
using Pommel.Reversi.Presentation.ViewModel.InGame;
using Pommel.Reversi.Presentation.ViewModel.Title;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Pommel.Reversi.Presentation.View.Title
{
    public interface IGameStartModal
    {
    }

    public sealed class GameStartModal : MonoBehaviour, IGameStartModal
    {
        private Text m_playerIdText = default;

        private Text m_playerNameText = default;

        private Text m_roomIdText = default;

        private Button m_createRoomButton = default;

        private Button m_entryRoomButton = default;

        [Inject]
        public void Construct(IGameViewModel gameState, ITitleViewModel titleViewModel)
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

            m_createRoomButton
                .OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ => gameState.CreateMatchingAsync(m_playerIdText.text, m_playerNameText.text).AsUniTask().ToObservable());

            m_entryRoomButton
                .OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ => gameState.EntryMatchingAsync(m_roomIdText.text, m_playerIdText.text, m_playerNameText.text).AsUniTask().ToObservable());

            titleViewModel.OnTapTitleAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ => gameObject.SetActive(true));

            gameObject.SetActive(false);
        }
    }
}
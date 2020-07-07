using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Pommel.Reversi.Infrastructure.Networking.Client;
using Pommel.Reversi.UseCase.InGame.Dto;
using UniRx;

namespace Pommel.Reversi.Presentation.Model.InGame
{
    public interface IGameModel
    {
        Task CreateMatchingAsync(string playerId, string playerName);

        Task EntryMatchingAsync(string matchingId, string playerId, string playerName);

        IObservable<ResultDto> OnResult { get; }

        IObservable<LaidDto> OnLaid { get; }
    }

    public sealed class GameModel : IGameModel
    {
        private readonly IInGameClient m_client;

        public GameModel(
            IInGameClientFactory inGameClientFactory
            )
        {
            m_client = inGameClientFactory.Create();
        }

        public IObservable<ResultDto> OnResult => throw new NotImplementedException();

        public IObservable<LaidDto> OnLaid => throw new NotImplementedException();

        public GameModel()
        {
            // todo 処理
            m_client.OnStartGameAsObservable()
                .Subscribe();
        }

        public async Task CreateMatchingAsync(string playerId, string playerName)
        {
            // todo dispose
            m_client.OnCreateMatchingAsObservable()
                .Subscribe(matchingId => m_client.JoinAsync(matchingId, playerId, playerName).AsUniTask().ToObservable());

            await m_client.CreateMatchingAsync(playerId, playerName);
        }

        public async Task EntryMatchingAsync(string matchingId, string playerId, string playerName)
        {
            await m_client.JoinAsync(matchingId, playerId, playerName);

            // todo dispose
            m_client.OnCreateGameAsObservable()
                .Subscribe(arg => m_client.StartAsync(arg.gameId));

            await m_client.CreateGameAsync(matchingId);
        }
    }
}
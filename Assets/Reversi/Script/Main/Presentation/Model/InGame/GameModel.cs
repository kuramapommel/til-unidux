using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.UseCase.InGame;
using Pommel.Reversi.UseCase.InGame.Dto;
using UniRx;
using _State = Pommel.Reversi.Domain.InGame.State;

namespace Pommel.Reversi.Presentation.Model.InGame
{
    public interface IGameModel
    {
        Task<IMatching> CreateMatchingAsync(string playerId, string playerName);

        Task<IMatching> EntryMatchingAsync(string matchingId, string playerId, string playerName);

        Task<IGame> CreateGameAsync(IMatching matching);

        Task<IGame> StartGameAsync();

        IObservable<ResultDto> OnResult { get; }

        IObservable<LaidDto> OnLaid { get; }
    }

    public sealed class GameModel : IGameModel
    {
        private readonly IGameResultService m_gameResultService;

        private readonly ILaidResultService m_laidResultService;

        private readonly ICreateMatchingUseCase m_createMatchingUseCase;

        private readonly IEntryMatchingUseCase m_entryMatchingUseCase;

        private readonly ICreateGameUseCase m_createGameUseCase;

        private readonly IStartGameUseCase m_startGameUseCase;

        private readonly IMessageReceiver m_messageReceiver;

        public GameModel(
            IGameResultService gameResultService,
            ILaidResultService laidResultService,
            ICreateMatchingUseCase createMatchingUseCase,
            IEntryMatchingUseCase entryMatchingUseCase,
            ICreateGameUseCase createGameUseCase,
            IStartGameUseCase startGameUseCase,
            IMessageBroker messageReceiver
            )
        {
            m_gameResultService = gameResultService;
            m_laidResultService = laidResultService;
            m_createMatchingUseCase = createMatchingUseCase;
            m_entryMatchingUseCase = entryMatchingUseCase;
            m_createGameUseCase = createGameUseCase;
            m_startGameUseCase = startGameUseCase;
            m_messageReceiver = messageReceiver;
        }

        public async Task<IMatching> CreateMatchingAsync(string playerId, string playerName) =>
            await m_createMatchingUseCase.Execute(playerId, playerName)
            .Match(
                Right: game => game,
                // todo error handling
                Left: error => throw error.Exception
            )
            .AsUniTask();

        public async Task<IMatching> EntryMatchingAsync(string matchingId, string playerId, string playerName) =>
            await m_entryMatchingUseCase.Execute(matchingId, playerId, playerName)
            .Match(
                Right: game => game,
                // todo error handling
                Left: error => throw error.Exception
            )
            .AsUniTask();

        public async Task<IGame> CreateGameAsync(IMatching matching) =>
            await m_createGameUseCase.Execute(matching)
            .Match(
                Right: game => game,
                // todo error handling
                Left: error => throw error.Exception
            )
            .AsUniTask();

        public async Task<IGame> StartGameAsync() =>
            await m_startGameUseCase.Execute()
            .Match(
                Right: game => game,
                // todo error handling
                Left: error => throw error.Exception
            )
            .AsUniTask();

        public IObservable<ResultDto> OnResult =>
            m_messageReceiver.Receive<ILaidPieceEvent>()
                .Where(message => message.Game.State == _State.GameSet)
                .SelectMany(message => m_gameResultService.FindById(message.Game.ResultId).AsUniTask().ToObservable());

        public IObservable<LaidDto> OnLaid =>
            m_messageReceiver.Receive<ILaidPieceEvent>()
            .SelectMany(message => m_laidResultService.FindById(message.Game.HistoryIds.LastOrDefault()).AsUniTask().ToObservable());
    }
}
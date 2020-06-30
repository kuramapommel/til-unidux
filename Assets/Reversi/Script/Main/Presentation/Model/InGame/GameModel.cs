using System;
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
        Task<IGame> CreateGameAsync();

        Task<IGame> StartGameAsync();

        IObservable<ResultDto> OnResult { get; }

        IObservable<ILaidPieceEvent> OnLaid { get; }
    }

    public sealed class GameModel : IGameModel
    {
        private readonly IGameResultService m_gameResultService;

        private readonly ICreateGameUseCase m_createGameUseCase;

        private readonly IStartGameUseCase m_startGameUseCase;

        private readonly IMessageReceiver m_messageReceiver;

        public GameModel(
            IGameResultService gameResultService,
            ICreateGameUseCase createGameUseCase,
            IStartGameUseCase startGameUseCase,
            IMessageBroker messageReceiver
            )
        {
            m_gameResultService = gameResultService;
            m_createGameUseCase = createGameUseCase;
            m_startGameUseCase = startGameUseCase;
            m_messageReceiver = messageReceiver;
        }

        public async Task<IGame> CreateGameAsync() =>
            await m_createGameUseCase.Execute()
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
                .SelectMany(message => m_gameResultService.FindById(message.Game.ResultId).ToObservable());

        public IObservable<ILaidPieceEvent> OnLaid =>
            m_messageReceiver.Receive<ILaidPieceEvent>();
    }
}
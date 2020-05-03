using System;
using System.Linq;
using Pommel.Reversi.Domain.InGame;
using UniRx.Async;

namespace Pommel.Reversi.UseCase.InGame
{
    public interface IStartGameUseCase
    {
        UniTask<IGame> Execute();
    }

    public sealed class StartGameUseCase : IStartGameUseCase
    {
        private readonly IGameRepository m_gameRepository;

        public StartGameUseCase(IGameRepository gameRepository) => m_gameRepository = gameRepository;

        public async UniTask<IGame> Execute()
        {
            var notyetGames = await m_gameRepository.Fetch(game => game.State == State.NotYet);
            var startedGame = notyetGames.FirstOrDefault()?.Start() ?? throw new ArgumentOutOfRangeException();
            return await m_gameRepository.Save(startedGame);
        }
    }
}
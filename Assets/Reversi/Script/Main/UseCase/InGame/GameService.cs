using Pommel.Reversi.Domain.InGame;
using UniRx.Async;

namespace Pommel.Reversi.UseCase.InGame
{
    public interface IGameService
    {
        UniTask<IGame> FetchPlaying();
    }
}
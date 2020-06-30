using Cysharp.Threading.Tasks;
using Pommel.Reversi.Domain.InGame;

namespace Pommel.Reversi.UseCase.InGame
{
    public interface IGameService
    {
        UniTask<IGame> FetchPlaying();
    }
}
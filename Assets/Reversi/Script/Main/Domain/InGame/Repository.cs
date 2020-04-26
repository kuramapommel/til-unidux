using UniRx.Async;

namespace Pommel.Reversi.Domain.InGame
{
    // 試合集約のrepository
    public interface IGameRepository
    {
        UniTask<IGame> FindById(string id);

        UniTask<IGame> Save(IGame game);
    }
}
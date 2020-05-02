using Pommel.Reversi.UseCase.InGame.Dto;
using UniRx.Async;

namespace Pommel.Reversi.UseCase.InGame
{
    public interface IGameResultService
    {
        UniTask<ResultDto> FindById(string id);
    }
}
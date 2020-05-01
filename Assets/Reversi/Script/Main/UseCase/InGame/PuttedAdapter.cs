using System.Collections.Generic;
using Pommel.Reversi.Domain.InGame;
using UniRx.Async;

namespace Pommel.Reversi.UseCase.InGame
{
    public interface IPuttedAdapter
    {
        // todo dto 型を用意して受け取る
        UniTask OnResult(ResultDto dto);

        // todo dto 型を用意して詰め替えて返す
        UniTask OnPut(PuttedDto dto);
    }

    public sealed class ResultDto
    {
    }

    public sealed class PuttedDto
    {
        public PuttedDto(IEnumerable<Stone> stones) { }
    }
}
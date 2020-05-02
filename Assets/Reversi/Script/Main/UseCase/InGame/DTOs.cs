using System.Collections.Generic;
using Pommel.Reversi.Domain.InGame;
using UniRx.Async;

namespace Pommel.Reversi.UseCase.InGame.Dto
{
    public sealed class ResultDto
    {
    }

    public sealed class PuttedDto
    {
        public IEnumerable<Stone> Stones { get; }

        public PuttedDto(IEnumerable<Stone> stones) => Stones = stones;
    }
}
using System.Collections.Generic;
using Pommel.Server.Domain.InGame;

namespace Pommel.Server.UseCase.InGame.Dto
{
    public sealed class ResultDto
    {
        public (int dark, int light) Count { get; }

        public Winner Winner { get; }

        public ResultDto(
            (int dark, int light) count,
            Winner winner
            )
        {
            Count = count;
            Winner = winner;
        }
    }
}
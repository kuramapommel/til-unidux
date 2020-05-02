using System.Collections.Generic;
using Pommel.Reversi.Domain.InGame;

namespace Pommel.Reversi.UseCase.InGame.Dto
{
    public sealed class ResultDto
    {
    }

    public sealed class LaidDto
    {
        public IEnumerable<Piece> Pieces { get; }

        public LaidDto(IEnumerable<Piece> pieces) => Pieces = pieces;
    }
}
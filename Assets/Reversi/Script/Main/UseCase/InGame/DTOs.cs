using System.Collections.Generic;
using Pommel.Reversi.Domain.InGame;

namespace Pommel.Reversi.UseCase.InGame.Dto
{
    public sealed class ResultDto
    {
        public string Id { get; }

        public (int dark, int light) Count { get; }

        public Winner Winner { get; }

        public ResultDto(
            string id,
            (int dark, int light) count,
            Winner winner
            )
        {
            Id = id;
            Count = count;
            Winner = winner;
        }
    }

    public sealed class LaidDto
    {
        public IEnumerable<Piece> Pieces { get; }

        public IPlayer NextTurnPlayer { get; }

        public LaidDto(
            IEnumerable<Piece> pieces,
            IPlayer nextTurnPlayer
            )
        {
            Pieces = pieces;
            NextTurnPlayer = nextTurnPlayer;
        }
    }
}
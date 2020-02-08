using System;
using System.Linq;
using Unidux;

namespace Pommel.Reversi
{
    [Serializable]
    public partial class State : StateBase
    {
        public readonly StoneStateElement[][] Stones = Enumerable.Range(0, 8)
            .Select(row => Enumerable.Range(0, 8)
                .Select(column =>
                {
                    switch ((row, column))
                    {
                        case var position when !(position.row == 3 || position.row == 4) || !(position.column == 3 || position.column == 4):
                            return new StoneStateElement();
                        case var position when position.row == 3 && position.column == 3:
                            return new StoneStateElement(StoneStateElement.State.White);
                        case var position when position.row == 3 && position.column == 4:
                            return new StoneStateElement(StoneStateElement.State.Black);
                        case var position when position.row == 4 && position.column == 3:
                            return new StoneStateElement(StoneStateElement.State.Black);
                        case var position when position.row == 4 && position.column == 4:
                            return new StoneStateElement(StoneStateElement.State.White);
                    }

                    return new StoneStateElement();
                })
            .ToArray())
        .ToArray();

        public readonly TurnStateElement Turn = new TurnStateElement();
    }

    public static class StoneStateElementExtension
    {
        public static bool CanPut(this StoneStateElement[][] source, int x, int y, bool isBlackTurn)
        {
            var upperLeftColor = source.ElementAtOrDefault(x - 1)?.ElementAtOrDefault(y - 1)?.Color ?? StoneStateElement.State.None;
            var upperColor = source.ElementAtOrDefault(x)?.ElementAtOrDefault(y - 1)?.Color ?? StoneStateElement.State.None;
            var upperRightColor = source.ElementAtOrDefault(x - 1)?.ElementAtOrDefault(y + 1)?.Color ?? StoneStateElement.State.None;
            var leftColor = source.ElementAtOrDefault(x - 1)?.ElementAtOrDefault(y)?.Color ?? StoneStateElement.State.None;
            var rightColor = source.ElementAtOrDefault(x + 1)?.ElementAtOrDefault(y)?.Color ?? StoneStateElement.State.None;
            var lowerLeftColor = source.ElementAtOrDefault(x + 1)?.ElementAtOrDefault(y - 1)?.Color ?? StoneStateElement.State.None;
            var lowerColor = source.ElementAtOrDefault(x)?.ElementAtOrDefault(y + 1)?.Color ?? StoneStateElement.State.None;
            var lowerRightColor = source.ElementAtOrDefault(x + 1)?.ElementAtOrDefault(y + 1)?.Color ?? StoneStateElement.State.None;

            var opponentColor = isBlackTurn ? StoneStateElement.State.White : StoneStateElement.State.Black;

            return upperLeftColor == opponentColor
                || upperColor == opponentColor
                || upperRightColor == opponentColor
                || leftColor == opponentColor
                || rightColor == opponentColor
                || lowerLeftColor == opponentColor
                || lowerColor == opponentColor
                || lowerRightColor == opponentColor;
        }
    }
}

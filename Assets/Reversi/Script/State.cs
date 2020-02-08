using System;
using Unidux;
using System.Linq;

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
        public static bool CannotPut(this StoneStateElement[][] source, int x, int y, bool isBlackTurn)
        {
            var hidariue = source.ElementAtOrDefault(x - 1)?.ElementAtOrDefault(y - 1);
            var ue = source.ElementAtOrDefault(x)?.ElementAtOrDefault(y - 1);
            var migiue = source.ElementAtOrDefault(x - 1)?.ElementAtOrDefault(y + 1);
            var hidari = source.ElementAtOrDefault(x - 1)?.ElementAtOrDefault(y);
            var migi = source.ElementAtOrDefault(x + 1)?.ElementAtOrDefault(y);
            var hidarishita = source.ElementAtOrDefault(x + 1)?.ElementAtOrDefault(y - 1);
            var shita = source.ElementAtOrDefault(x)?.ElementAtOrDefault(y + 1);
            var migishita = source.ElementAtOrDefault(x + 1)?.ElementAtOrDefault(y + 1);

            return (hidariue?.Color ?? StoneStateElement.State.None) == StoneStateElement.State.None
                && (ue?.Color ?? StoneStateElement.State.None) == StoneStateElement.State.None
                && (migiue?.Color ?? StoneStateElement.State.None) == StoneStateElement.State.None
                && (hidari?.Color ?? StoneStateElement.State.None) == StoneStateElement.State.None
                && (migi?.Color ?? StoneStateElement.State.None) == StoneStateElement.State.None
                && (hidarishita?.Color ?? StoneStateElement.State.None) == StoneStateElement.State.None
                && (shita?.Color ?? StoneStateElement.State.None) == StoneStateElement.State.None
                && (migishita?.Color ?? StoneStateElement.State.None) == StoneStateElement.State.None;
        }
    }
}

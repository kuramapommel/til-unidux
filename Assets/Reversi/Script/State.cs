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
    }
}

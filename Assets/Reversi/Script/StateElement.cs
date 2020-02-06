using System;
using Unidux;

namespace Pommel.Reversi
{
    [Serializable]
    public sealed class StoneStateElement : StateElement
    {
        public State Color { get; set; }

        public bool IsBlackTurn { get; set; } = true;

        public StoneStateElement(State color = State.None) => Color = color;

        public enum State
        {
            None,
            White,
            Black
        }
    }
}
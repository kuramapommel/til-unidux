using System;
using Unidux;

namespace Pommel.Reversi.Presentation.Scene.InGame
{
    [Serializable]
    public sealed class StoneStateElement : StateElement
    {
        public State Color { get; set; }

        public StoneStateElement(State color = State.None) => Color = color;

        public enum State
        {
            None,
            White,
            Black
        }
    }

    [Serializable]
    public sealed class TurnStateElement : StateElement
    {
        public bool IsBlackTurn { get; set; } = true;
    }
}
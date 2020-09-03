using System;
using Unidux;

namespace Pommel
{
    using Reversi.Reducks.Title;

    public interface IState
    {
        TitleState Title { get; set; }
    }

    [Serializable]
    public sealed class State : StateBase, IState
    {
        public TitleState Title { get; set; } = Reversi.Reducks.Title.State.Element;
    }
}
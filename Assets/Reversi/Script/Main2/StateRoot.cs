using System;
using Unidux;

namespace Pommel
{
    using Reversi.Reducks.Title;
    using Reversi.Reducks.InGame;

    public interface IProps
    {
        Reversi.Reducks.Title.IProps Title { get; }
        Reversi.Reducks.InGame.IProps InGame { get; }
    }

    public interface IState
    {
        TitleState Title { get; }

        InGameState InGame { get; }
    }

    [Serializable]
    public sealed class State : StateBase, IState, IProps
    {
        TitleState IState.Title { get; } = Reversi.Reducks.Title.State.Element;
        InGameState IState.InGame { get; } = Reversi.Reducks.InGame.State.Element;

        Reversi.Reducks.Title.IProps IProps.Title => Reversi.Reducks.Title.State.Props;
        Reversi.Reducks.InGame.IProps IProps.InGame => Reversi.Reducks.InGame.State.Props;
    }
}
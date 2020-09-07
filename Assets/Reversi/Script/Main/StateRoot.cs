using System;
using Unidux;
using Unidux.SceneTransition;
using static Pommel.Reversi.Domain.Scene.ValueObjects;

namespace Pommel
{
    using Reversi.Reducks.InGame;
    using Reversi.Reducks.Title;

    public interface IProps
    {
        Reversi.Reducks.Title.IProps Title { get; }

        Reversi.Reducks.InGame.IProps InGame { get; }
    }

    public abstract class StateRoot : StateBase
    {
        public abstract TitleState Title { get; }

        public abstract InGameState InGame { get; }

        public abstract SceneState<Scene> Scene { get; set; }

        public abstract PageState<Page> Page { get; set; }
    }

    [Serializable]
    public sealed class State : StateRoot, IProps
    {
        public override TitleState Title { get; } = Reversi.Reducks.Title.State.Element;

        public override InGameState InGame { get; } = Reversi.Reducks.InGame.State.Element;

        public override SceneState<Scene> Scene { get; set; } = new SceneState<Scene>();

        public override PageState<Page> Page { get; set; } = new PageState<Page>();

        Reversi.Reducks.Title.IProps IProps.Title => Reversi.Reducks.Title.State.Props;

        Reversi.Reducks.InGame.IProps IProps.InGame => Reversi.Reducks.InGame.State.Props;
    }
}
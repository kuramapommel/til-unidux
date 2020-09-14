using System;
using Pommel.Reversi.Domain.Scene;
using Unidux.SceneTransition;

namespace Pommel.Reversi.Reducks.Transition
{
    public static class Actions
    {
        public static Func<IPageData, PageDuck<ValueObjects.Page, ValueObjects.Scene>.PushAction> ToInGameAction = payload => PageDuck<ValueObjects.Page, ValueObjects.Scene>.ActionCreator.Push(ValueObjects.Page.InGame, payload);

        public static Func<IPageData, PageDuck<ValueObjects.Page, ValueObjects.Scene>.PushAction> ToTitleAction = payload => PageDuck<ValueObjects.Page, ValueObjects.Scene>.ActionCreator.Push(ValueObjects.Page.Title, payload);
    }
}
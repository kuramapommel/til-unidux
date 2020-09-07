using System;
using Unidux.SceneTransition;
using Pommel.Reversi.Domain.Scene;

namespace Pommel.Reversi.Reducks.Scene
{
    public static class Actions
    {
        public static Func<IPageData, PageDuck<ValueObjects.Page, ValueObjects.Scene>.PushAction> ToInGameAction = payload => PageDuck<ValueObjects.Page, ValueObjects.Scene>.ActionCreator.Push(ValueObjects.Page.InGame, payload);

        public static Func<IPageData, PageDuck<ValueObjects.Page, ValueObjects.Scene>.PushAction> ToTitleAction = payload => PageDuck<ValueObjects.Page, ValueObjects.Scene>.ActionCreator.Push(ValueObjects.Page.Title, payload);
    }
}
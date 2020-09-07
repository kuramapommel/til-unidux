using System;
using Unidux;
using UniRx;
using UnityEngine;

namespace Pommel
{
    public interface IDispatcher
    {
        object Dispatch<TAction>(TAction action);
    }

    public interface IStateAsObservableCreator
    {
        IObservable<T> Create<T>(Component takeUntilDisable, Func<State, T> selector);
    }

    public static class Unidux
    {
        public static IDispatcher Dispatcher => m_instance.Value;

        public static IObservable<T> CrateObservable<T>(Component takeUntilDisable, Func<State, T> selector) => m_instance.Value.Create(takeUntilDisable, selector);

        private static readonly Lazy<Impl> m_instance = new Lazy<Impl>(() => new Impl(
            InitialState,
            Reducers,
            Middlewares
            ));

        private static State InitialState => new State();

        private static IReducer[] Reducers => new IReducer[]
        {
            new Reversi.Reducks.Title.Reducer(),
            new Reversi.Reducks.InGame.Reducer(),
        };

        private static Middleware[] Middlewares => new Middleware[] { };

        private sealed class Impl : IDisposable, IDispatcher, IStateAsObservableCreator
        {
            private readonly IDisposable m_disposable;

            private readonly Store<State> m_store;

            public IObservable<T> Create<T>(Component takeUntilDisable, Func<State, T> selector) => m_store.Subject
                .TakeUntilDisable(takeUntilDisable)
                .StartWith(m_store.State)
                .Select(selector)
                .Publish()
                .RefCount();

            object IDispatcher.Dispatch<TAction>(TAction action) => m_store.Dispatch(action);

            public Impl(State state, IReducer[] reducers, Middleware[] middlewares)
            {
                m_store = new Store<State>(state, reducers);
                m_store.ApplyMiddlewares(middlewares);
                m_disposable = Observable.EveryUpdate().Subscribe(_ => m_store.Update());
            }

            void IDisposable.Dispose()
            {
                m_disposable.Dispose();
            }
        }
    }
}
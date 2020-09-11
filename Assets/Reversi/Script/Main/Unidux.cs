using System;
using Pommel.Middlewares;
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
        IObservable<T> Create<T>(Component takeUntilDisable, Func<State, bool> predicate, Func<State, T> selector);
    }

    public static class Unidux
    {
        public static IDispatcher Dispatcher => m_instance.Value;

        public static IStateAsObservableCreator StateAsObservableCreator => m_instance.Value;

        public static IProps Props => m_instance.Value.State;

        public static StateRoot State => m_instance.Value.State;

        private static readonly Lazy<Impl> m_instance = new Lazy<Impl>(() => new Impl());

        private sealed class Impl : IDisposable, IDispatcher, IStateAsObservableCreator
        {
            private readonly IDisposable m_disposable;

            private readonly Store<State> m_store;

            public State State => m_store.State;

            public IObservable<T> Create<T>(Component takeUntilDisable, Func<State, bool> predicate, Func<State, T> selector) => m_store.Subject
                .TakeUntilDisable(takeUntilDisable)
                .Where(predicate)
                .StartWith(m_store.State)
                .Select(selector)
                .Publish()
                .RefCount();

            object IDispatcher.Dispatch<TAction>(TAction action) => m_store.Dispatch(action);

            public Impl()
            {
                var initialState = new State();

                var reducers = new IReducer[]
                {
                    new Reversi.Reducks.Title.Reducer(),
                    new Reversi.Reducks.InGame.Reducer(),
                    new Reversi.Reducks.Scene.PageReducer(),
                    new Reversi.Reducks.Scene.SceneReducer(),
                };

                var middlewares = new Middleware[]
                {
                    new Thunk(this).Middleware,
                };


                m_store = new Store<State>(initialState, reducers);
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
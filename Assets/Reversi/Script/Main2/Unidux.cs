using System;
using Unidux;
using UniRx;

namespace Pommel
{
    public interface IState
    {
        int Count { get; }
    }

    [Serializable]
    public sealed class State : StateBase, IState
    {
        public int Count => 0;
    }

    public static class Unidux
    {
        public interface IUnidux : IStoreAccessor, IDisposable
        {
            State State { get; }

            Subject<State> Subject { get; }

            Store<State> Store { get; }

            object Dispatch<TAction>(TAction action);
        }

        public static IUnidux Instance => m_unidux.Value;

        private static readonly Lazy<IUnidux> m_unidux = new Lazy<IUnidux>(() => new Impl(
            InitialState,
            Reducers
            ));

        private static State InitialState => new State();

        private static IReducer[] Reducers => new IReducer[] { };

        private sealed class Impl : IUnidux
        {
            private readonly IDisposable m_disposable;

            IStoreObject IStoreAccessor.StoreObject => Store;

            State IUnidux.State => Store.State;

            Subject<State> IUnidux.Subject => Store.Subject;

            public Store<State> Store { get; }

            object IUnidux.Dispatch<TAction>(TAction action) => Store.Dispatch(action);

            public Impl(State state, params IReducer[] reducers)
            {
                Store = new Store<State>(state, reducers);
                m_disposable = Observable.EveryUpdate().Subscribe(_ => Store.Update());
            }

            void IDisposable.Dispose()
            {
                m_disposable.Dispose();
            }
        }
    }
}
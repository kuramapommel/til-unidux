using System;
using Unidux;

namespace Pommel.Middlewares
{
    public sealed class Thunk
    {
        private readonly IDispatcher m_dispatcher;

        public Thunk(IDispatcher dispatcher)
        {
            m_dispatcher = dispatcher;
        }

        public Func<Func<object, object>, Func<object, object>> Middleware(IStoreObject storeObject) =>
            next => action => (action is Func<IDispatcher, object> function)
                ? function(m_dispatcher)
                : next(action);
    }
}
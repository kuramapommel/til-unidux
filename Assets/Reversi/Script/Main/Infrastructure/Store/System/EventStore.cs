using System;
using System.Collections.Generic;
using Pommel.Reversi.UseCase.System;
using UniRx.Async;

namespace Pommel.Reversi.Infrastructure.Store.System
{
    public sealed class EventBroker : IEventBroker
    {
        private readonly IDictionary<Type, IEventSubscriber> m_store = new Dictionary<Type, IEventSubscriber>();

        public UniTask<EventMessage> SendMessage<EventMessage>(EventMessage message) where EventMessage : IEventMessage => m_store.TryGetValue(typeof(EventMessage), out var subscriber)
                ? subscriber.ReceivedMessage(message)
                    .ContinueWith(() => message)
                : throw new IndexOutOfRangeException();

        public UniTask RegisterSubscriber<EventMessage>(IEventSubscriber subscriber) where EventMessage : IEventMessage => UniTask.Run(() =>
        {
            var key = typeof(EventMessage);
            if (m_store.ContainsKey(key)) m_store.Remove(key);
            m_store.Add(key, subscriber);
        });
    }
}
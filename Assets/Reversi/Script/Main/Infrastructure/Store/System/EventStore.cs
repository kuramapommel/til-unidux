using System;
using System.Collections.Generic;
using Pommel.Reversi.UseCase.Shared;
using UniRx.Async;

namespace Pommel.Reversi.Infrastructure.Store.System
{
    public sealed class EventBroker : IEventBroker
    {
        private readonly IDictionary<int, IEventSubscriber> m_store = new Dictionary<int, IEventSubscriber>();

        public UniTask<EventMessage> SendMessage<EventMessage>(EventMessage message) where EventMessage : IEventMessage
        {
            var hash = typeof(EventMessage).GetHashCode();
            return m_store.TryGetValue(hash, out var subscriber)
                ? subscriber.ReceivedMessage(message)
                    .ContinueWith(() => message)
                : throw new IndexOutOfRangeException();
        }

        public UniTask RegisterSubscriber<EventMessage>(IEventSubscriber subscriber) where EventMessage : IEventMessage
        {
            var hash = typeof(EventMessage).GetHashCode();

            if (m_store.ContainsKey(hash)) m_store.Remove(hash);
            m_store.Add(hash, subscriber);
            return UniTask.CompletedTask;
        }
    }
}
using System;
using System.Reactive.Subjects;

namespace Pommel.Server.Component.Reactive
{
    public interface IMessageBroker<T> : IMessagePublisher<T>, IMessageReciever<T>, IDisposable
        where T : IMessage
    {
    }

    public interface IMessagePublisher<T>
    {
        void Publish(T message);
    }

    public interface IMessageReciever<T>
    {
        IObservable<T> OnRecieve();
    }

    public interface IMessage
    {
    }

    public static class MessageBroker<T>
            where T : IMessage
    {
        private sealed class Impl : IMessageBroker<T>
        {
            private readonly ISubject<T> m_onRecieve = new Subject<T>();

            public void Publish(T message)
            {
                m_onRecieve.OnNext(message);
            }

            public IObservable<T> OnRecieve() => m_onRecieve;

            void IDisposable.Dispose()
            {
                m_onRecieve.OnCompleted();
            }
        }

        private static readonly Lazy<IMessageBroker<T>> m_impl = new Lazy<IMessageBroker<T>>(() => new Impl());

        public static IMessageBroker<T> Default => m_impl.Value;
    }
}
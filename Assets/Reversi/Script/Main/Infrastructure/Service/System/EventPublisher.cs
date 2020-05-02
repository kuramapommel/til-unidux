using Pommel.Reversi.UseCase.Shared;
using UniRx.Async;

namespace Pommel.Reversi.Infrastructure.Service.System
{
    public sealed class EventPublisher : IEventPublisher
    {
        private readonly IEventBroker m_eventBroker;

        public EventPublisher(IEventBroker eventBroker)
        {
            m_eventBroker = eventBroker;
        }

        public UniTask<EventMessage> Publish<EventMessage>(EventMessage eventMessage) where EventMessage : IEventMessage => m_eventBroker.SendMessage(eventMessage);
    }
}
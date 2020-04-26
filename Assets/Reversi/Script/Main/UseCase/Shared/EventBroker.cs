using UniRx.Async;

namespace Pommel.Reversi.UseCase.Shared
{
    public interface IEventBroker
    {
        UniTask<DomainEvent> SendMessage<DomainEvent>(DomainEvent domainEvent) where DomainEvent : IDomainEvent;

        UniTask RegisterSubscriber<DomainEvent>(DomainEvent domainEvent, IEventSubscriber subscriber) where DomainEvent : IDomainEvent;
    }
}
using UniRx.Async;

namespace Pommel.Reversi.UseCase.Shared
{
    public interface IEventPublisher
    {
        UniTask<DomainEvent> Publish<DomainEvent>(DomainEvent domainEvent) where DomainEvent : IDomainEvent;
    }
}
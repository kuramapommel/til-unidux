using UniRx.Async;

namespace Pommel.Reversi.UseCase.InGame
{
    public interface IEventPublisher
    {
        UniTask<DomainEvent> Publish<DomainEvent>(DomainEvent domainEvent) where DomainEvent : IDomainEvent;
    }

    public interface IDomainEvent
    {
    }

    public sealed class PutStoneEvent : IDomainEvent
    {

    }
}
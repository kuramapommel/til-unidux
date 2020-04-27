using UniRx.Async;

namespace Pommel.Reversi.UseCase.Shared
{
    public interface IEventSubscriber
    {
        UniTask ReceivedMessage<DomainEvent>(DomainEvent domainEvent) where DomainEvent : IEventMessage;
    }
}
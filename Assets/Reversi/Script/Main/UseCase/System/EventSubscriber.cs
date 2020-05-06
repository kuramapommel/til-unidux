using UniRx.Async;

namespace Pommel.Reversi.UseCase.System
{
    public interface IEventSubscriber
    {
        UniTask ReceivedMessage<DomainEvent>(DomainEvent domainEvent) where DomainEvent : IEventMessage;
    }
}
using UniRx.Async;

namespace Pommel.Reversi.UseCase.Shared
{
    public interface IEventBroker
    {
        UniTask<EventMessage> SendMessage<EventMessage>(EventMessage message) where EventMessage : IEventMessage;

        UniTask RegisterSubscriber<EventMessage>(IEventSubscriber subscriber) where EventMessage : IEventMessage;
    }
}
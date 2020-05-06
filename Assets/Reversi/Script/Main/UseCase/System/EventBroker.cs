using UniRx.Async;

namespace Pommel.Reversi.UseCase.System
{
    public interface IEventBroker
    {
        UniTask<EventMessage> SendMessage<EventMessage>(EventMessage message) where EventMessage : IEventMessage;

        UniTask RegisterSubscriber<EventMessage>(IEventSubscriber subscriber) where EventMessage : IEventMessage;
    }
}
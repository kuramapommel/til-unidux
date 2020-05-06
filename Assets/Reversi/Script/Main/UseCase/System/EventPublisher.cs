using UniRx.Async;

namespace Pommel.Reversi.UseCase.System
{
    public interface IEventPublisher
    {
        UniTask<EventMessage> Publish<EventMessage>(EventMessage eventMessage) where EventMessage : IEventMessage;
    }
}
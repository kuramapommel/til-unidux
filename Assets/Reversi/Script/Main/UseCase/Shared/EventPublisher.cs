using UniRx.Async;

namespace Pommel.Reversi.UseCase.Shared
{
    public interface IEventPublisher
    {
        UniTask<EventMessage> Publish<EventMessage>(EventMessage eventMessage) where EventMessage : IEventMessage;
    }
}
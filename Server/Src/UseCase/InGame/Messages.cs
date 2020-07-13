using Pommel.Server.Component.Reactive;

namespace Pommel.Server.UseCase.InGame.Message
{
    public interface IResultMessage : IMessage
    {
        string ResultId { get; }
    }

    public sealed class ResultMessage : IResultMessage
    {
        public string ResultId { get; }

        public ResultMessage(string resultId)
        {
            ResultId = resultId;
        }
    }
}
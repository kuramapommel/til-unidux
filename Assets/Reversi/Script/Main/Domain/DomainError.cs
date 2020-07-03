using System;

namespace Pommel.Reversi.Domain
{
    // todo めっちゃ仮、ちゃんとエラーハンドリングように作成する
    public interface IError
    {
        Exception Exception { get; }
        string Message { get; }
    }

    public sealed class DomainError : IError
    {
        public Exception Exception { get; }

        public string Message { get; }

        public DomainError(Exception exception, string message = null)
        {
            Exception = exception;
            Message = message ?? exception.Message;
        }
    }
}
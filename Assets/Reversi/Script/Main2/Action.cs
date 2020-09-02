using System;

namespace Pommel
{
    public interface IAction<E> where E : Enum
    {
        E Type { get; }
    }

    public sealed class ActionCommand<E, T> : IAction<E> where E : Enum
    {
        public E Type { get; }

        public T Payload { get; }

        public ActionCommand(E type, T payload)
        {
            Type = type;
            Payload = payload;
        }
    }
}
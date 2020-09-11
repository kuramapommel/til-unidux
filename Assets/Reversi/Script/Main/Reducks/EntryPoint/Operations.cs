using System;
using System.Threading.Tasks;
using static Pommel.Reversi.Reducks.Scene.Actions;

namespace Pommel.Reversi.Reducks.EntryPoint
{
    public interface IOperation
    {
        Func<Func<IDispatcher, Task>> ToTitle { get; }
    }

    public static class Operation
    {
        public interface IFactory
        {
            IOperation Create();
        }

        public sealed class Impl : IOperation
        {
            public Func<Func<IDispatcher, Task>> ToTitle { get; }

            public Impl(
            )
            {
                ToTitle = () => async dispatcher => dispatcher.Dispatch(ToTitleAction(default));
            }
        }
    }
}
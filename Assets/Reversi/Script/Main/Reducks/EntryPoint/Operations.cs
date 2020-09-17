using System;
using System.Threading.Tasks;
using static Pommel.Reversi.Reducks.Transition.Actions;

namespace Pommel.Reversi.Reducks.EntryPoint
{
    public static class Operations
    {
        public interface ILoadableTitle
        {
            Func<Func<IDispatcher, Task>> LoadTitle { get; }
        }
    }

    public static class OperationImpls
    {
        public sealed class LoadTitleOperation : Operations.ILoadableTitle
        {
            public Func<Func<IDispatcher, Task>> LoadTitle { get; } = () => async dispatcher => dispatcher.Dispatch(ToTitleAction(default));
        }
    }
}
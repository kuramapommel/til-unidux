using System;
using System.Threading.Tasks;
using static Pommel.Reversi.Reducks.Scene.Actions;

namespace Pommel.Reversi.Reducks.EntryPoint
{
    public interface IOperation
    {
        Func<Task> ToTitle { get; }
    }

    public static class Opration
    {
        private sealed class Impl : IOperation
        {
            public Func<Task> ToTitle { get; }

            public Impl(
                IDispatcher dispatcher,
                Pommel.IProps props
            )
            {
                ToTitle = async () => dispatcher.Dispatch(ToTitleAction(default));
            }
        }
    }
}
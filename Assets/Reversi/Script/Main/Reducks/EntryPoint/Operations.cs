using System;
using System.Threading.Tasks;
using static Pommel.Reversi.Reducks.Scene.Actions;

namespace Pommel.Reversi.Reducks.EntryPoint
{
    public interface IOperation
    {
        Func<Task> ToTitle { get; }
    }

    public sealed class Opration : IOperation
    {
        public Func<Task> ToTitle { get; }

        public Opration(
            IDispatcher dispatcher,
            IProps props
        )
        {
            ToTitle = async () => dispatcher.Dispatch(ToTitleAction(default));
        }
    }
}
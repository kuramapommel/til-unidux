using System;
using System.Threading.Tasks;
using static Pommel.Reversi.Reducks.InGame.Actions;

namespace Pommel.Reversi.Reducks.InGame
{
    public interface IOperation
    {
        Func<ValueObject.Point, Task> PutStone { get; }

        Func<ValueObject.Stone[], string, Task> RefreshAndNextTurn { get; }
    }

    public static class Opration
    {
        private sealed class Impl : IOperation
        {
            public Func<ValueObject.Point, Task> PutStone { get; }

            public Func<ValueObject.Stone[], string, Task> RefreshAndNextTurn { get; }

            public Impl(IDispatcher dispatcher, Pommel.IProps props)
            {
                PutStone = async point =>
                {
                    // todo サーバーにアクセスして、point に石を置く
                };

                RefreshAndNextTurn = async (stones, nextTurnPlayerId) =>
                {
                    dispatcher.Dispatch(RefreshBoardAction(stones));
                    dispatcher.Dispatch(SwitchTurnAction(nextTurnPlayerId));
                };
            }
        }
    }
}
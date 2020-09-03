using System;
using System.Threading.Tasks;
using static Pommel.Reversi.Reducks.InGame.Actions;

namespace Pommel.Reversi.Reducks.InGame
{
    public interface IOperation
    {
        Func<ValueObject.Point, Task> PutStone { get; }
    }

    public static class Opration
    {
        private sealed class Impl : IOperation
        {
            public Func<ValueObject.Point, Task> PutStone { get; }
            public Impl(IDispatcher dispatcher, Pommel.IProps props)
            {
                PutStone = async (point) =>
                {
                    // todo サーバーにアクセスして、point に石を置く
                    // todo 取得した board 情報を dispatch して store に保存する
                    var stones = props.InGame.Board.Stones;
                    dispatcher.Dispatch(RefreshBoardAction(stones));
                };
            }
        }
    }
}
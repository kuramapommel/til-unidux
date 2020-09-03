using System;
using System.Threading.Tasks;
using static Pommel.Reversi.Reducks.Title.Actions;

namespace Pommel.Reversi.Reducks.Title
{
    public interface IOperation
    {
        Func<Task> OpenGameStartModal { get; }

        Func<string, Task> InputPlayerId { get; }

        Func<string, Task> InputPlayerName { get; }

        Func<string, Task> InputRoomId { get; }

        Func<Task> CreateRoom { get; }

        Func<Task> EntryRoom { get; }
    }

    public static class Opration
    {
        private sealed class Impl : IOperation
        {
            public Func<Task> OpenGameStartModal { get; }

            public Func<string, Task> InputPlayerId { get; }

            public Func<string, Task> InputPlayerName { get; }

            public Func<string, Task> InputRoomId { get; }

            public Func<Task> CreateRoom { get; }

            public Func<Task> EntryRoom { get; }

            public Impl(IDispatcher dispatcher, IProps props)
            {
                OpenGameStartModal = async () => dispatcher.Dispatch(OpenGameStartModalAction(true));
                InputPlayerId = async (id) => dispatcher.Dispatch(InputPlayerIdAction(id));
                InputPlayerName = async (name) => dispatcher.Dispatch(InputPlayerNameAction(name));
                InputRoomId = async (roomId) => dispatcher.Dispatch(InputRoomIdAction(roomId));
                CreateRoom = async () =>
                {
                    var player = props.Player;

                    // todo サーバにアクセスして room を作成する
                    // todo 作成された room 情報を dispatch して store に保存する
                };
                EntryRoom = async () =>
                {
                    // todo props から room id を取得する
                    // todo サーバにアクセスして room に entry する
                    // todo 取得した room 情報を dispatch して store に保存する
                };
            }
        }
    }
}
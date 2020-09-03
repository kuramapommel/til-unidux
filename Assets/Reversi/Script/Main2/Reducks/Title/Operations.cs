using System;
using System.Threading.Tasks;
using static Pommel.Reversi.Reducks.Title.Actions;
using static Pommel.Reversi.Reducks.InGame.Actions;
using static Pommel.Reversi.Reducks.InGame.ValueObject;

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

            public Impl(IDispatcher dispatcher, Pommel.IProps props)
            {
                OpenGameStartModal = async () => dispatcher.Dispatch(OpenGameStartModalAction(true));
                InputPlayerId = async id => dispatcher.Dispatch(InputPlayerIdAction(id));
                InputPlayerName = async name => dispatcher.Dispatch(InputPlayerNameAction(name));
                InputRoomId = async roomId => dispatcher.Dispatch(InputRoomIdAction(roomId));
                CreateRoom = async () =>
                {
                    var player = props.Title.Player;

                    // todo サーバにアクセスして room を作成する
                    var room = new Room();
                    dispatcher.Dispatch(CreateRoomAction(room));
                };
                EntryRoom = async () =>
                {
                    var (playerInfo, roomId) = (props.Title.Player, props.Title.RoomId);

                    // todo サーバにアクセスして room に entry する
                    // todo 取得した room 情報を dispatch して store に保存する
                    var room = new Room();
                    dispatcher.Dispatch(CreateRoomAction(room));

                    // todo サーバにアクセスして game を作成する
                    var secondPlayer = new Room.Player(playerInfo.Id, playerInfo.Name, Stone.Color.Light, false);
                    dispatcher.Dispatch(EntryGameAction(secondPlayer));
                };
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using static Pommel.Reversi.Domain.InGame.ValueObjects;
using static Pommel.Reversi.Reducks.InGame.Actions;
using static Pommel.Reversi.Reducks.Scene.Actions;
using static Pommel.Reversi.Reducks.Title.Actions;
using IInGameClient = Pommel.Reversi.Domain.InGame.IClient;

namespace Pommel.Reversi.Reducks.Title
{
    public interface IOperation
    {
        Func<Task> OpenGameStartModal { get; }

        Func<string, string, Task> CreateRoom { get; }

        Func<string, string, string, Task> EntryRoom { get; }

        Func<Game, Task> StartGame { get; }
    }

    public static class Opration
    {
        private sealed class Impl : IOperation
        {
            public Func<Task> OpenGameStartModal { get; }

            public Func<string, string, Task> CreateRoom { get; }

            public Func<string, string, string, Task> EntryRoom { get; }

            public Func<Game, Task> StartGame { get; }

            public Impl(IDispatcher dispatcher, Pommel.IProps props, IInGameClient client)
            {
                OpenGameStartModal = async () => dispatcher.Dispatch(OpenGameStartModalAction(true));
                CreateRoom = async (playerId, playerName) =>
                {
                    await client.ConnectAsync().AsUniTask();

                    var roomId = await client.CreateRoomAsync().AsUniTask();
                    await client.EntryRoomAsync(roomId, playerId, playerName).AsUniTask();

                    dispatcher.Dispatch(CreateRoomAction(
                        new Room(
                        roomId,
                        new Room.Player(
                            playerId,
                            playerName,
                            Stone.Color.Dark,
                            true
                        ),
                        props.InGame.Room.SecondPlayer
                        )));
                };
                EntryRoom = async (playerId, playerName, roomId) =>
                {
                    await client.ConnectAsync().AsUniTask();

                    var room = await client.FindRoomById(roomId).AsUniTask();
                    await client.EntryRoomAsync(roomId, playerId, playerName).AsUniTask();

                    var player = new Room.Player(playerId, playerName, Stone.Color.Light, false);
                    dispatcher.Dispatch(CreateRoomAction(
                        new Room(
                            room.RoomId,
                            room.FirstPlayer,
                            player
                        )));
                    dispatcher.Dispatch(EntryRoomAction(player));

                    await client.CreateGameAsync(roomId).AsUniTask();
                };
                StartGame = async game =>
                {
                    dispatcher.Dispatch(RefreshGameAction(game));
                    dispatcher.Dispatch(ToInGameAction(default));
                };
            }
        }
    }
}
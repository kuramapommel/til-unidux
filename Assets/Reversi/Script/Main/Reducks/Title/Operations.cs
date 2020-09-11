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
        Func<Func<IDispatcher, Task>> OpenGameStartModal { get; }

        Func<string, string, Func<IDispatcher, Task>> CreateRoom { get; }

        Func<string, string, string, Func<IDispatcher, Task>> EntryRoom { get; }

        Func<Game, Func<IDispatcher, Task>> StartGame { get; }
    }

    public static class Operation
    {
        public interface IFactory
        {
            IOperation Create();
        }

        public sealed class Impl : IOperation
        {
            public Func<Func<IDispatcher, Task>> OpenGameStartModal { get; }

            public Func<string, string, Func<IDispatcher, Task>> CreateRoom { get; }

            public Func<string, string, string, Func<IDispatcher, Task>> EntryRoom { get; }

            public Func<Game, Func<IDispatcher, Task>> StartGame { get; }

            public Impl(Pommel.IProps props, IInGameClient client)
            {
                OpenGameStartModal = () => async dispatcher => dispatcher.Dispatch(OpenGameStartModalAction(true));
                CreateRoom = (playerId, playerName) => async dispatcher =>
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
                EntryRoom = (playerId, playerName, roomId) => async dispatcher =>
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
                StartGame = game => async dispatcher =>
                {
                    dispatcher.Dispatch(RefreshGameAction(game));
                    dispatcher.Dispatch(ToInGameAction(default));
                };
            }
        }
    }

}
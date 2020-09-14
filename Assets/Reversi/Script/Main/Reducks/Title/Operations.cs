using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using static Pommel.Reversi.Domain.InGame.ValueObjects;
using static Pommel.Reversi.Reducks.InGame.Actions;
using static Pommel.Reversi.Reducks.Title.Actions;
using static Pommel.Reversi.Reducks.Transition.Actions;
using IInGameClient = Pommel.Reversi.Domain.InGame.IClient;

namespace Pommel.Reversi.Reducks.Title
{
    public static class Operations
    {
        public interface IOpenableGameStartModal
        {
            Func<Func<IDispatcher, Task>> OpenGameStartModal { get; }
        }

        public interface ICreatableRoom
        {
            Func<string, string, Func<IDispatcher, Task>> CreateRoom { get; }
        }

        public interface IEnteralbleRoom
        {
            Func<string, string, string, Func<IDispatcher, Task>> EnterRoom { get; }
        }

        public interface IStartableGame
        {
            Func<Game, Func<IDispatcher, Task>> StartGame { get; }
        }
    }

    public static class OperationImpls
    {
        public sealed class OpenGameStartModalOperation: Operations.IOpenableGameStartModal
        {
            public Func<Func<IDispatcher, Task>> OpenGameStartModal { get; } = () => async dispatcher => dispatcher.Dispatch(OpenGameStartModalAction(true));
        }

        public sealed class CreateRoomOperation : Operations.ICreatableRoom
        {
            public Func<string, string, Func<IDispatcher, Task>> CreateRoom { get; }

            public CreateRoomOperation(Pommel.IProps props, IInGameClient client)
            {
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
            }
        }

        public sealed class EnterRoomOperation : Operations.IEnteralbleRoom
        {
            public Func<string, string, string, Func<IDispatcher, Task>> EnterRoom { get; }

            public EnterRoomOperation(IInGameClient client)
            {
                EnterRoom = (playerId, playerName, roomId) => async dispatcher =>
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
            }
        }

        public sealed class StartGameOperation : Operations.IStartableGame
        {
            public Func<Game, Func<IDispatcher, Task>> StartGame { get; } = game => async dispatcher =>
            {
                dispatcher.Dispatch(RefreshGameAction(game));
                dispatcher.Dispatch(ToInGameAction(default));
            };
        }
    }
}
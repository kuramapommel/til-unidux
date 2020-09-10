using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Pommel.Reversi.Domain.InGame;
using static Pommel.Reversi.Reducks.InGame.Actions;
using static Pommel.Reversi.Reducks.Scene.Actions;
using IInGameClient = Pommel.Reversi.Domain.InGame.IClient;

namespace Pommel.Reversi.Reducks.InGame
{
    public interface IOperation
    {
        Func<ValueObjects.Point, Task> PutStone { get; }

        Func<ValueObjects.Game, Task> RefreshAndNextTurn { get; }

        Func<Task> ReturnToTitle { get; }
    }

    public sealed class Opration : IOperation
    {
        public Func<ValueObjects.Point, Task> PutStone { get; }

        public Func<ValueObjects.Game, Task> RefreshAndNextTurn { get; }

        public Func<Task> ReturnToTitle { get; }

        public Opration(IDispatcher dispatcher, Pommel.IProps props, IInGameClient client)
        {
            PutStone = async point => await client.PutStoneAsync(point.X, point.Y).AsUniTask();

            RefreshAndNextTurn = async game =>
            {
                dispatcher.Dispatch(RefreshGameAction(game));
            };
            ReturnToTitle = async () =>
            {
                await client.DisconnectAsync().AsUniTask();
                dispatcher.Dispatch(ToTitleAction(default));
            };
        }
    }
}
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
        Func<ValueObjects.Point, Func<IDispatcher, Task>> PutStone { get; }

        Func<ValueObjects.Game, Func<IDispatcher, Task>> RefreshAndNextTurn { get; }

        Func<Func<IDispatcher, Task>> ReturnToTitle { get; }
    }

    public static class Operation
    {
        public interface IFactory
        {
            IOperation Create();
        }

        public sealed class Impl : IOperation
        {
            public Func<ValueObjects.Point, Func<IDispatcher, Task>> PutStone { get; }

            public Func<ValueObjects.Game, Func<IDispatcher, Task>> RefreshAndNextTurn { get; }

            public Func<Func<IDispatcher, Task>> ReturnToTitle { get; }

            public Impl(Pommel.IProps props, IInGameClient client)
            {
                PutStone = point => async dispatcher => await client.PutStoneAsync(point.X, point.Y).AsUniTask();

                RefreshAndNextTurn = game => async dispatcher =>
                {
                    dispatcher.Dispatch(RefreshGameAction(game));
                };
                ReturnToTitle = () => async dispatcher =>
                {
                    await client.DisconnectAsync().AsUniTask();
                    dispatcher.Dispatch(ToTitleAction(default));
                };
            }
        }
    }
}
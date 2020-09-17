using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Pommel.Reversi.Domain.InGame;
using static Pommel.Reversi.Reducks.InGame.Actions;
using static Pommel.Reversi.Reducks.Transition.Actions;
using IInGameClient = Pommel.Reversi.Domain.InGame.IClient;

namespace Pommel.Reversi.Reducks.InGame
{
    public static class Operations
    {
        public interface IPutableStone
        {
            Func<ValueObjects.Point, Func<IDispatcher, Task>> PutStone { get; }
        }

        public interface IRefreshableAndNextTurn
        {
            Func<ValueObjects.Game, Func<IDispatcher, Task>> RefreshAndNextTurn { get; }
        }

        public interface IReturnableToTile
        {
            Func<Func<IDispatcher, Task>> ReturnToTitle { get; }
        }
    }

    public static class OperationImpls
    {
        public sealed class PutStoneOperation : Operations.IPutableStone
        {
            public Func<ValueObjects.Point, Func<IDispatcher, Task>> PutStone { get; }

            public PutStoneOperation(IInGameClient client)
            {
                PutStone = point => async dispatcher => await client.PutStoneAsync(point.X, point.Y).AsUniTask();
            }
        }

        public sealed class RefreshAndNextTurnOperation : Operations.IRefreshableAndNextTurn
        {
            public Func<ValueObjects.Game, Func<IDispatcher, Task>> RefreshAndNextTurn { get; } = game => async dispatcher => dispatcher.Dispatch(RefreshGameAction(game));
        }

        public sealed class ReturnToTitleOperation : Operations.IReturnableToTile
        {
            public Func<Func<IDispatcher, Task>> ReturnToTitle { get; }

            public ReturnToTitleOperation(IInGameClient client)
            {
                ReturnToTitle = () => async dispatcher =>
                {
                    await client.DisconnectAsync().AsUniTask();
                    dispatcher.Dispatch(ToTitleAction(default));
                };
            }
        }
    }
}
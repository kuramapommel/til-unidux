using Pommel.Reversi.Domain.InGame;
using UniRx;

namespace Pommel.Reversi.UseCase.InGame
{
    public sealed class LaidPieceMessageBroker : MessageBroker
    {

    }

    public interface ILaidPieceEvent
    {
        IGame Game { get; }
    }

    public sealed class LaidPieceEvent : ILaidPieceEvent
    {
        public IGame Game { get; }

        public LaidPieceEvent(IGame game) => Game = game;
    }
}
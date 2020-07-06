using System;

namespace Pommel.Reversi.UseCase.InGame
{
    public interface ILaidPieceMessageReciever
    {
        IObservable<ILaidPieceEvent> OnResult { get; }

        IObservable<ILaidPieceEvent> OnLaid { get; }

        IObservable<LayEvent> OnLay { get; }
    }
}
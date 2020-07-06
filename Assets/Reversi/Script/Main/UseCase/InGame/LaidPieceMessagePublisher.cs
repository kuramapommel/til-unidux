using System;

namespace Pommel.Reversi.UseCase.InGame
{
    public interface ILaidPieceMessagePublisher
    {
        void Join();

        void Lay(string gameId, int x, int y);
    }
}
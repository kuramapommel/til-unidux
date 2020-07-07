using System;

namespace Pommel.Server.UseCase.InGame
{
    public interface ILaidPieceMessagePublisher
    {
        void Join();

        void Lay(string gameId, int x, int y);
    }
}
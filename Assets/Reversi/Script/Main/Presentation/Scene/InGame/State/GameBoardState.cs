using System.Collections.Generic;
using System.Linq;
using Pommel.Reversi.Domain.InGame;

namespace Pommel.Reversi.Presentation.Scene.InGame.State
{
    public interface IGameBoardState
    {
        IEnumerable<IPieceState> Pieces { get; }

        void Refresh(IEnumerable<Piece> pieces);
    }

    public sealed class GameBoardState : IGameBoardState
    {
        public IEnumerable<IPieceState> Pieces { get; }

        public GameBoardState(IEnumerable<IPieceState> pieces)
        {
            Pieces = pieces.ToList();
        }

        public void Refresh(IEnumerable<Piece> pieces)
        {
            foreach (var (piece, state) in pieces
                .Join(
                    Pieces,
                    pieceEntity => pieceEntity.Point,
                    state => state.Point,
                    (pieceEntity, state) => (pieceEntity, state)
                ))
            {
                state.SetColor(piece.Color.Convert());
            }
        }
    }
}
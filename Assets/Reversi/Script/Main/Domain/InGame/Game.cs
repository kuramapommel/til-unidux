using System;
using System.Collections.Generic;
using System.Linq;

namespace Pommel.Reversi.Domain.InGame
{
    // 試合集約
    public interface IGame
    {
        // todo 型作る
        string Id { get; }

        // todo 型作る
        string ResultId { get; }

        State State { get; }

        Turn Turn { get; }

        IEnumerable<Piece> Pieces { get; }

        IGame LayPiece(Point point);
    }

    public sealed class Game : IGame
    {
        public string Id { get; }

        public string ResultId { get; } = string.Empty;

        public State State { get; } = State.NotYet;

        public Turn Turn { get; } = Turn.FirstPlayer;

        public IEnumerable<Piece> Pieces { get; } = Enumerable.Range(0, 8)
            .SelectMany(x => Enumerable.Range(0, 8)
                .Select(y => new Piece(new Point(x, y))));

        public IGame LayPiece(Point point)
        {
            var (playerColor, opponentColor) = Turn == Turn.FirstPlayer
                ? (Color.Light, Color.Dark)
                : (Color.Dark, Color.Light);

            var aroundOpponentPieces = point.AdjacentPoints
                    .Join(
                        Pieces,
                        aroundPoint => (aroundPoint, opponentColor),
                        piece => (piece.Point, piece.Color),
                        (aroundPoint, piece) => piece
                    )
                    .ToArray();

            // 周りに敵の駒がない場合は置けない
            if (!aroundOpponentPieces.Any()) throw new ArgumentException();

            var flipTarget = aroundOpponentPieces
                .Select(opponentPiece => point.CreateSpecifiedVector(opponentPiece.Point)
                    .Join(
                        Pieces,
                        vectorPiece => (vectorPiece.X, vectorPiece.Y),
                        piece => (piece.Point.X, piece.Point.Y),
                        (vectorPiece, piece) => piece)
                    .TakeWhile(piece => piece.Color == playerColor || piece.Color == Color.None)
                    .ToArray())
                .Where(vectorPieces => vectorPieces.Any(piece => piece.Color == playerColor))
                .SelectMany(vectorPieces => vectorPieces.Select(piece => piece.SetColor(playerColor)))
                .ToArray();

            var flippedPieces = Pieces
                .GroupJoin(
                    flipTarget,
                    piece => (piece.Point.X, piece.Point.Y),
                    flipped => (flipped.Point.X, flipped.Point.Y),
                    (piece, flippeds) => (piece, flippeds)
                )
                .SelectMany(
                    aggregate => aggregate.flippeds.DefaultIfEmpty(),
                    (aggregate, flipped) => flipped ?? aggregate.piece
                )
                .ToArray();

            // todo 相手側が駒を置くことができるかチェック、できる場合はターンきりかえて return

            // todo 相手側が駒を置けない場合はプレイヤー側が続けて駒を置くことができるかチェック、できる場合はターンを保持したまま return

            // todo 両者置けない場合はゲームセットとして return
            return new Game(Id, ResultId, State, Turn, flippedPieces);
        }

        public Game(string id)
        {
            Id = id;
        }

        private Game(string id, string resultId, State state, Turn turn, IEnumerable<Piece> pieces)
        {
            Id = id;
            ResultId = resultId;
            State = state;
            Turn = turn;
            Pieces = pieces;
        }
    }

    public enum State
    {
        NotYet,
        Playing,
        GameSet
    }

    public enum Turn
    {
        FirstPlayer,
        SecondPlayer
    }
}
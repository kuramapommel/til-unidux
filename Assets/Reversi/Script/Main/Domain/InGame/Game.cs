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

        IGame Start();
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

        public Game(string id)
        {
            Id = id;
        }

        public IGame LayPiece(Point point)
        {
            // すでにおいてあった場合は置けない
            if (Pieces.First(piece => piece.Point.X == point.X && piece.Point.Y == point.Y).Color != Color.None) return this;

            var (playerColor, opponentColor, nextTurn) = Turn == Turn.FirstPlayer
                ? (Color.Dark, Color.Light, Turn.SecondPlayer)
                : (Color.Light, Color.Dark, Turn.FirstPlayer);

            var aroundOpponentPieces = point.AdjacentPoints
                    .Join(
                        Pieces,
                        aroundPoint => (aroundPoint, opponentColor),
                        piece => (piece.Point, piece.Color),
                        (aroundPoint, piece) => piece
                    )
                    .ToArray();

            // 周りに敵の駒がない場合は置けない
            if (!aroundOpponentPieces.Any()) return this;

            var flipTarget = aroundOpponentPieces
                .SelectMany(opponentPiece =>
                {
                    var vectorPieces = point.CreateSpecifiedVector(opponentPiece.Point)
                        .Join(
                            Pieces,
                            vectorPiece => (vectorPiece.X, vectorPiece.Y),
                            piece => (piece.Point.X, piece.Point.Y),
                            (vectorPiece, piece) => piece)
                        .ToArray();
                    var targets = vectorPieces.TakeWhile(piece => piece.Color != playerColor);
                    var betweenOther = vectorPieces.ElementAtOrDefault(targets.Count());

                    if (targets.Any(piece => piece.Color == Color.None)
                        || betweenOther == null
                        || betweenOther.Color != playerColor
                    ) return Enumerable.Empty<Piece>();

                    return targets.Select(piece => piece.SetColor(playerColor));
                })
                .ToArray();

            // 裏返す対象の駒がない場合は置けない
            if (!flipTarget.Any()) return this;

            var flippedPieces = Pieces
                .GroupJoin(
                    flipTarget,
                    piece => (piece.Point.X, piece.Point.Y),
                    flipped => (flipped.Point.X, flipped.Point.Y),
                    (piece, flippeds) => (piece, flippeds)
                )
                .SelectMany(
                    aggregate => aggregate.flippeds.DefaultIfEmpty(),
                    (aggregate, flipped) => flipped
                        ?? (aggregate.piece.Point.X == point.X && aggregate.piece.Point.Y == point.Y
                            ? aggregate.piece.SetColor(playerColor)
                            : aggregate.piece)
                )
                .ToArray();

            // todo 相手側が駒を置くことができるかチェック、できる場合はターンきりかえて return

            // todo 相手側が駒を置けない場合はプレイヤー側が続けて駒を置くことができるかチェック、できる場合はターンを保持したまま return

            // todo 両者置けない場合はゲームセットとして return
            return new Game(Id, ResultId, State, nextTurn, flippedPieces);
        }

        public IGame Start() => new Game(Id, ResultId, State.Playing, Turn,
            Pieces.Select(piece =>
            {
                if (Point.InitialDarkPoints.Contains(piece.Point)) return piece.SetColor(Color.Dark);
                if (Point.InitialLightPoints.Contains(piece.Point)) return piece.SetColor(Color.Light);
                return piece;
            })
            .ToArray());

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
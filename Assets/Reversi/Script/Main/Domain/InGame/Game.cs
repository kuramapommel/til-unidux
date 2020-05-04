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

        Player TurnPlayer { get; }

        IEnumerable<Piece> Pieces { get; }

        IGame LayPiece(Point point);

        IGame Start();
    }

    public sealed class Game : IGame
    {
        public string Id { get; }

        public string ResultId { get; } = string.Empty;

        public State State { get; } = State.NotYet;

        public Player TurnPlayer { get; } = Player.First;

        public IEnumerable<Piece> Pieces { get; } = Enumerable.Range(0, 8)
            .SelectMany(x => Enumerable.Range(0, 8)
                .Select(y => new Piece(new Point(x, y))));

        public Game(string id, string resultId)
        {
            Id = id;
            ResultId = resultId;
        }

        public IGame LayPiece(Point point)
        {
            // 配置済みの箇所を指定された場合は例外
            if (Pieces.First(piece => piece.Point.X == point.X && piece.Point.Y == point.Y).Color != Color.None) throw new ArgumentException();

            var (playerColor, opponentColor, opponent) = TurnPlayer == Player.First
                ? (Color.Dark, Color.Light, Player.Second)
                : (Color.Light, Color.Dark, Player.First);

            var flipTargets = this.CreateFlipTargets(TurnPlayer, point, Pieces);

            // 反転対象の駒が存在しない場合は例外
            if (!flipTargets.Any()) throw new ArgumentException();

            // 判定処理
            var flippedPieces = Pieces
                .GroupJoin(
                    flipTargets,
                    piece => (piece.Point.X, piece.Point.Y),
                    target => (target.Point.X, target.Point.Y),
                    (piece, targets) => (piece, targets)
                )
                .SelectMany(
                    aggregate => aggregate.targets.DefaultIfEmpty(),
                    (aggregate, target) => target?.SetColor(playerColor)
                        ?? (aggregate.piece.Point.X == point.X && aggregate.piece.Point.Y == point.Y
                            ? aggregate.piece.SetColor(playerColor)
                            : aggregate.piece)
                )
                .ToArray();

            var nonePoints = flippedPieces.Where(flippedPiece => flippedPiece.Color == Color.None).ToArray();

            // 相手側が駒を置くことができるかチェック、できる場合はターンきりかえて return
            if (nonePoints.Any(nonePoint => this.IsValide(opponent, nonePoint.Point, flippedPieces))) return new Game(Id, ResultId, State, opponent, flippedPieces);

            // 相手側が駒を置けない場合はプレイヤー側が続けて駒を置くことができるかチェック、できる場合はターンを保持したまま return
            if (nonePoints.Any(nonePoint => this.IsValide(TurnPlayer, nonePoint.Point, flippedPieces))) return new Game(Id, ResultId, State, TurnPlayer, flippedPieces);

            // 両者置けない場合はゲームセットとして return
            return new Game(Id, ResultId, State.GameSet, opponent, flippedPieces);
        }

        public IGame Start() => new Game(Id, ResultId, State.Playing, TurnPlayer,
            Pieces.Select(piece =>
            {
                if (Point.InitialDarkPoints.Contains(piece.Point)) return piece.SetColor(Color.Dark);
                if (Point.InitialLightPoints.Contains(piece.Point)) return piece.SetColor(Color.Light);
                return piece;
            })
            .ToArray());

        private Game(string id, string resultId, State state, Player turn, IEnumerable<Piece> pieces)
        {
            Id = id;
            ResultId = resultId;
            State = state;
            TurnPlayer = turn;
            Pieces = pieces;
        }
    }

    public static class GameExtension
    {
        public static bool IsValide(this IGame source, Player player, Point point, IEnumerable<Piece> pieces)
        {
            if (pieces.First(piece => piece.Point.X == point.X && piece.Point.Y == point.Y).Color != Color.None) return false;

            return source.CreateFlipTargets(player, point, pieces).Any();
        }

        public static IEnumerable<Piece> CreateFlipTargets(this IGame source, Player player, Point layPoint, IEnumerable<Piece> basePieces)
        {
            var (playerColor, opponentColor) = player == Player.First
                ? (Color.Dark, Color.Light)
                : (Color.Light, Color.Dark);

            return layPoint.AdjacentPoints
                    .Join(
                        basePieces,
                        aroundPoint => (aroundPoint, opponentColor),
                        piece => (piece.Point, piece.Color),
                        (aroundPoint, piece) => piece
                    )
                    .SelectMany(opponentPiece =>
                    {
                        var vectorPieces = layPoint.CreateSpecifiedVectorPoints(opponentPiece.Point)
                            .Join(
                                basePieces,
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

                        return targets;
                    })
                    .ToArray();
        }
    }

    public enum State
    {
        NotYet,
        Playing,
        GameSet
    }

    public enum Player
    {
        First,
        Second
    }
}
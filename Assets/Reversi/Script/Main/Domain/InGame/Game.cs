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

        IPlayer FirstPlayer { get; }

        IPlayer SecondPlayer { get; }

        Turn Turn { get; }

        IEnumerable<Piece> Pieces { get; }

        IGame MakeMatch(IPlayer first, IPlayer second);

        IGame LayPiece(Point point);

        IGame Start();
    }

    public interface IGameFactory
    {
        IGame Create(string id, string resultId);
    }

    public sealed class Game : IGame
    {
        public string Id { get; }

        public string ResultId { get; } = string.Empty;

        public State State { get; } = State.NotYet;

        public IPlayer FirstPlayer { get; } = Player.None;

        public IPlayer SecondPlayer { get; } = Player.None;

        public Turn Turn { get; } = Turn.First;

        public IEnumerable<Piece> Pieces { get; } = Enumerable.Range(0, 8)
            .SelectMany(x => Enumerable.Range(0, 8)
                .Select(y => new Piece(new Point(x, y))));

        public Game(string id, string resultId)
        {
            Id = id;
            ResultId = resultId;
        }

        public IGame MakeMatch(IPlayer first, IPlayer second) =>
            State == State.NotYet
            ? new Game(Id, ResultId,
                State.MakedMatch,
                first,
                second,
                Turn,
                Pieces
                )
            // todo 妥当な例外に置き換える
            : throw new Exception();

        public IGame LayPiece(Point point)
        {
            // todo 妥当な例外に置き換える
            if (State != State.Playing) throw new Exception();

            // 配置済みの箇所を指定された場合は例外
            if (Pieces.First(piece => piece.Point.X == point.X && piece.Point.Y == point.Y).Color != Color.None) throw new ArgumentException();

            var (playerColor, opponentColor, opponent) = Turn == Turn.First
                ? (Color.Dark, Color.Light, Turn.Second)
                : (Color.Light, Color.Dark, Turn.First);

            var flipTargets = this.CreateFlipTargets(Turn, point, Pieces);

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
            if (nonePoints.Any(nonePoint => this.IsValid(opponent, nonePoint.Point, flippedPieces))) return new Game(Id, ResultId, State, FirstPlayer, SecondPlayer, opponent, flippedPieces);

            // 相手側が駒を置けない場合はプレイヤー側が続けて駒を置くことができるかチェック、できる場合はターンを保持したまま return
            if (nonePoints.Any(nonePoint => this.IsValid(Turn, nonePoint.Point, flippedPieces))) return new Game(Id, ResultId, State, FirstPlayer, SecondPlayer, Turn, flippedPieces);

            // 両者置けない場合はゲームセットとして return
            return new Game(Id, ResultId, State.GameSet, FirstPlayer, SecondPlayer, opponent, flippedPieces);
        }

        public IGame Start() => new Game(Id, ResultId, State.Playing, FirstPlayer, SecondPlayer, Turn,
            Pieces.Select(piece =>
            {
                if (Point.InitialDarkPoints.Contains(piece.Point)) return piece.SetColor(Color.Dark);
                if (Point.InitialLightPoints.Contains(piece.Point)) return piece.SetColor(Color.Light);
                return piece;
            })
            .ToArray());

        private Game(string id, string resultId, State state, IPlayer firstPlayer, IPlayer secondPlayer, Turn turn, IEnumerable<Piece> pieces)
        {
            Id = id;
            ResultId = resultId;
            State = state;
            FirstPlayer = firstPlayer;
            SecondPlayer = secondPlayer;
            Turn = turn;
            Pieces = pieces;
        }
    }

    public static class GameExtension
    {
        public static bool IsValid(this IGame source, Turn player, Point point, IEnumerable<Piece> pieces)
        {
            if (pieces.First(piece => piece.Point.X == point.X && piece.Point.Y == point.Y).Color != Color.None) return false;

            return source.CreateFlipTargets(player, point, pieces).Any();
        }

        public static IEnumerable<Piece> CreateFlipTargets(this IGame source, Turn player, Point layPoint, IEnumerable<Piece> basePieces)
        {
            var (playerColor, opponentColor) = player == Turn.First
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
        MakedMatch,
        Playing,
        GameSet
    }

    public enum Turn
    {
        First,
        Second
    }
}
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

        IEnumerable<Stone> Stones { get; }

        IGame PutStone(Point point);
    }

    public sealed class Game : IGame
    {
        public string Id { get; }

        public string ResultId { get; } = string.Empty;

        public State State { get; } = State.NotYet;

        public Turn Turn { get; } = Turn.FirstPlayer;

        public IEnumerable<Stone> Stones { get; } = Enumerable.Range(0, 8)
            .SelectMany(x => Enumerable.Range(0, 8)
                .Select(y => new Stone(new Point(x, y))));

        public IGame PutStone(Point point)
        {
            var (playerColor, opponentColor) = Turn == Turn.FirstPlayer
                ? (Color.Light, Color.Dark)
                : (Color.Dark, Color.Light);

            var aroundOpponentPieces = point.AdjacentPoints
                    .Join(
                        Stones,
                        aroundPoint => (aroundPoint, opponentColor),
                        stone => (stone.Point, stone.Color),
                        (aroundPoint, stone) => stone
                    )
                    .ToArray();

            // 周りに敵の駒がない場合は置けない
            if (!aroundOpponentPieces.Any()) throw new ArgumentException();

            var flipTarget = aroundOpponentPieces
                .Select(opponentPiece => point.CreateSpecifiedVector(opponentPiece.Point)
                    .Join(
                        Stones,
                        vectorPiece => (vectorPiece.X, vectorPiece.Y),
                        stone => (stone.Point.X, stone.Point.Y),
                        (vectorPiece, stone) => stone)
                    .TakeWhile(stone => stone.Color == playerColor || stone.Color == Color.None)
                    .ToArray())
                .Where(vectorStones => vectorStones.Any(stone => stone.Color == playerColor))
                .SelectMany(vectorStones => vectorStones.Select(stone => stone.SetColor(playerColor)))
                .ToArray();

            var flippedPieces = Stones
                .GroupJoin(
                    flipTarget,
                    stone => (stone.Point.X, stone.Point.Y),
                    flipped => (flipped.Point.X, flipped.Point.Y),
                    (stone, flippeds) => (stone, flippeds)
                )
                .SelectMany(
                    aggregate => aggregate.flippeds.DefaultIfEmpty(),
                    (aggregate, flipped) => flipped ?? aggregate.stone
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

        private Game(string id, string resultId, State state, Turn turn, IEnumerable<Stone> stones)
        {
            Id = id;
            ResultId = resultId;
            State = state;
            Turn = turn;
            Stones = stones;
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
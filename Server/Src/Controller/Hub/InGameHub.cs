using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MagicOnion;
using MagicOnion.Server.Hubs;
using Pommel.Api.Hubs;
using Pommel.Server.Component.Reactive;
using Pommel.Server.UseCase.InGame;
using Pommel.Server.UseCase.InGame.Message;
using _Game = Pommel.Api.Protocol.InGame.Game;
using _Piece = Pommel.Api.Protocol.InGame.Piece;

namespace Pommel.Server.Controller.Hub
{
    public sealed class InGameHub : StreamingHubBase<IInGameHub, IInGameReceiver>, IInGameHub
    {
        private readonly IStartGameUseCase m_startGameUseCase;

        private readonly ILayPieceUseCase m_layPieceUseCase;

        private readonly IMessageReciever<IResultMessage> m_resultMessageReciver;

        private IGroup m_room;

        private string m_playerId = string.Empty;

        private string m_playerName = string.Empty;

        public InGameHub(
            IStartGameUseCase startGameUseCase,
            ILayPieceUseCase layPieceUseCase,
            IMessageBroker<IResultMessage> resultMessageBroker,
            IGameResultService gameResultService
            )
        {
            m_startGameUseCase = startGameUseCase;
            m_layPieceUseCase = layPieceUseCase;
            m_resultMessageReciver = resultMessageBroker;

            // dispose
            m_resultMessageReciver.OnRecieve()
                .Subscribe(message => gameResultService.FindById(message.ResultId)
                    .Match(
                        Right: resultDto => Broadcast(m_room).OnResult(
                            resultDto.Count.dark,
                            resultDto.Count.light,
                            (int)resultDto.Winner
                            ),
                        // todo エラーの内容を見て正しくハンドリング
                        Left: error => throw new ReturnStatusException((Grpc.Core.StatusCode)99, error.Message)
                    ));
        }

        public async Task JoinAsync(string matchingId, string playerId, string playerName)
        {
            Logger.Debug($"called JoinAsync: matching id is {matchingId}, player id is {playerId}, player name is {playerName}");
            m_playerId = playerId;
            m_playerName = playerName;
            m_room = await Group.AddAsync(matchingId);
            // todo join usecase を実装して呼ぶ

            BroadcastExceptSelf(m_room).OnJoin(matchingId, m_playerId, m_playerName);
        }

        public async Task StartGameAsync(string gameId) =>
            await m_startGameUseCase.Execute(gameId)
                .Match(
                    Right: game => Broadcast(m_room).OnStartGame(
                        game.NextTurnPlayerId,
                        new _Game
                        {
                            Id = game.Id,
                            Pieces = game.Pieces
                                .Select(piece => new _Piece
                                {
                                    X = piece.Point.X,
                                    Y = piece.Point.Y,
                                    Color = (int)piece.Color
                                })
                                .ToArray()
                        }),
                    // todo エラーの内容を見て正しくハンドリング
                    Left: error => throw new ReturnStatusException((Grpc.Core.StatusCode)99, error.Message)
                );

        public async Task LayAsync(string gameId, int x, int y) =>
            await m_layPieceUseCase.Execute(gameId, x, y)
                .Match(
                    Right: game => Broadcast(m_room).OnLay(
                        game.NextTurnPlayerId,
                        new _Game
                        {
                            Id = game.Id,
                            Pieces = game.Pieces
                                .Select(piece => new _Piece
                                {
                                    X = piece.Point.X,
                                    Y = piece.Point.Y,
                                    Color = (int)piece.Color
                                })
                                .ToArray()
                        }),
                    // todo エラーの内容を見て正しくハンドリング
                    Left: error => throw new ReturnStatusException((Grpc.Core.StatusCode)99, error.Message)
                );
    }
}
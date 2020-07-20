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

        private readonly ICreateMatchingUseCase m_createMatchingUseCase;

        private readonly IEntryMatchingUseCase m_entryMatchingUseCase;

        private readonly ICreateGameUseCase m_createGameUseCase;

        private readonly IMessageReciever<IResultMessage> m_resultMessageReciver;

        private IGroup m_room;

        private string m_playerId = string.Empty;

        private string m_playerName = string.Empty;

        public InGameHub(
            IStartGameUseCase startGameUseCase,
            ILayPieceUseCase layPieceUseCase,
            ICreateMatchingUseCase createMatchingUseCase,
            IEntryMatchingUseCase entryMatchingUseCase,
            ICreateGameUseCase createGameUseCase,
            IMessageBroker<IResultMessage> resultMessageBroker,
            IGameResultService gameResultService
            )
        {
            m_startGameUseCase = startGameUseCase;
            m_layPieceUseCase = layPieceUseCase;
            m_createMatchingUseCase = createMatchingUseCase;
            m_resultMessageReciver = resultMessageBroker;
            m_entryMatchingUseCase = entryMatchingUseCase;
            m_createGameUseCase = createGameUseCase;

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

        async Task IInGameHub.CreateMatchingAsync(string playerId, string playerName) =>
            await m_createMatchingUseCase.Execute(playerId, playerName)
                    .Match(
                        Right: async matching =>
                        {
                            m_playerId = playerId;
                            m_playerName = playerName;
                            m_room = await Group.AddAsync(matching.Id);

                            Broadcast(m_room).OnJoin(matching.Id, m_playerId, m_playerName, string.Empty, string.Empty);
                        },
                        // todo エラーの内容を見て正しくハンドリング
                        Left: error => throw new ReturnStatusException((Grpc.Core.StatusCode)99, error.Message)
                    )
                    .Unwrap();

        async Task IInGameHub.EntryMatchingAsync(string matchingId, string playerId, string playerName) =>
            await m_entryMatchingUseCase.Execute(matchingId, playerId, playerName)
                .Match(
                    Right: async matching =>
                    {
                            m_playerId = playerId;
                            m_playerName = playerName;
                            m_room = await Group.AddAsync(matching.Id);

                            Broadcast(m_room).OnJoin(matching.Id, matching.FirstPlayer.Id, matching.FirstPlayer.Name, matching.SecondPlayer.Id, matching.SecondPlayer.Name);
                    },
                    // todo エラーの内容を見て正しくハンドリング
                    Left: error => throw new ReturnStatusException((Grpc.Core.StatusCode)99, error.Message)
                )
                .Unwrap();

        async Task IInGameHub.CreateGameAsync(string matchingId) =>
            await m_createGameUseCase.Execute(matchingId)
                .Match(
                    Right: game => Broadcast(m_room).OnCreateGame(game.Id, matchingId),
                    // todo エラーの内容を見て正しくハンドリング
                    Left: error => throw new ReturnStatusException((Grpc.Core.StatusCode)99, error.Message)
                );

        async Task IInGameHub.StartGameAsync(string gameId) =>
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

        async Task IInGameHub.LayAsync(string gameId, int x, int y) =>
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
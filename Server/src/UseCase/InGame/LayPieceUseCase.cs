using System;
using LanguageExt;
using Pommel.Server.Domain;
using Pommel.Server.Domain.InGame;
using static LanguageExt.Prelude;
using _State = Pommel.Server.Domain.InGame.State;

namespace Pommel.Server.UseCase.InGame
{
    public interface ILayPieceUseCase
    {
        EitherAsync<IError, IGame> Execute(string gameId, int x, int y);
    }

    public sealed class LayPieceUseCase : ILayPieceUseCase
    {
        private readonly IGameRepository m_gameRepository;

        private readonly IResultCalculator m_resultCalculator;

        private readonly ISwitchTurnDispatcher m_switchTurnDispatcher;

        public LayPieceUseCase(
            IGameRepository gameRepository,
            IResultCalculator resultCalculator,
            ISwitchTurnDispatcher switchTurnDispatcher
            )
        {
            m_gameRepository = gameRepository;
            m_resultCalculator = resultCalculator;
            m_switchTurnDispatcher = switchTurnDispatcher;
        }

        public EitherAsync<IError, IGame> Execute(string gameId, int x, int y) =>
                from point in RightAsync<IError, Point>(new Point(x, y))
                from game in m_gameRepository.FindById(gameId).ToAsync()
                from _ in game.IsValid(game.Turn, point, game.Pieces)
                    ? RightAsync<IError, bool>(true)
                    : LeftAsync<IError, bool>(new DomainError(new ArgumentOutOfRangeException(), "その場所には置くことができません")) // todo invalid 時の妥当なエラー処理
                from laid in Try(() => game.LayPiece(point))
                    .ToEitherAsync()
                    .MapLeft(e => new DomainError(e) as IError)
                from __ in m_switchTurnDispatcher.Dispatch(laid).ToAsync()
                from saved in m_gameRepository.Save(laid).ToAsync()
                    .Map(savedGame =>
                    {
                        if (savedGame.State != _State.GameSet) return savedGame;

                        var _ = m_resultCalculator.Calculate(savedGame).ToAsync();
                        return savedGame;
                    })
                select saved;
    }
}
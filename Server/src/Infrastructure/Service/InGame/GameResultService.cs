using LanguageExt;
using Pommel.Server.Domain;
using Pommel.Server.Domain.InGame;
using Pommel.Server.UseCase.InGame;
using Pommel.Server.UseCase.InGame.Dto;
using static LanguageExt.Prelude;

namespace Pommel.Server.Infrastructure.Service.InGame
{
    public sealed class GameResultService : IGameResultService
    {
        private readonly IResultRepository m_repository;

        public GameResultService(IResultRepository repository) => m_repository = repository;

        public EitherAsync<IError, ResultDto> FindById(string id) =>
            from gameResult in m_repository.FindById(id).ToAsync()
            from resultDto in Try(() => new ResultDto(gameResult.Count, gameResult.Winner))
                    .ToEitherAsync()
                    .MapLeft(e => new DomainError(e) as IError)
            select resultDto;
    }
}
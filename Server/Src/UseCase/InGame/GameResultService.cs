using LanguageExt;
using Pommel.Server.Domain;
using Pommel.Server.UseCase.InGame.Dto;

namespace Pommel.Server.UseCase.InGame
{
    public interface IGameResultService
    {
        EitherAsync<IError, ResultDto> FindById(string id);
    }
}
using System.Threading.Tasks;
using Pommel.Server.UseCase.InGame.Dto;

namespace Pommel.Server.UseCase.InGame
{
    public interface IGameResultService
    {
        Task<ResultDto> FindById(string id);
    }
}
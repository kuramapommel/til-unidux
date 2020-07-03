using System.Threading.Tasks;
using Pommel.Reversi.UseCase.InGame.Dto;

namespace Pommel.Reversi.UseCase.InGame
{
    public interface IGameResultService
    {
        Task<ResultDto> FindById(string id);
    }
}
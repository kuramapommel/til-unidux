using System.Threading.Tasks;
using Pommel.Reversi.UseCase.InGame.Dto;

namespace Pommel.Reversi.UseCase.InGame
{
    public interface ILaidResultService
    {
        Task<LaidDto> FindById(string gameId);
    }
}
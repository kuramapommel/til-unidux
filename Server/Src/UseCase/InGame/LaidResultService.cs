using System.Threading.Tasks;
using Pommel.Server.UseCase.InGame.Dto;

namespace Pommel.Server.UseCase.InGame
{
    public interface ILaidResultService
    {
        Task<LaidDto> FindById(string gameId);
    }
}
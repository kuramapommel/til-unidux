using LanguageExt;
using Pommel.Server.Domain;
using Pommel.Server.Domain.InGame;

namespace Pommel.Server.UseCase.InGame
{
    public interface IFindRoomUseCase
    {
        EitherAsync<IError, IRoom> Execute(string roomId);
    }

    public sealed class FindRoomUseCase : IFindRoomUseCase
    {
        private readonly IRoomRepository m_roomRepository;

        public FindRoomUseCase(
            IRoomRepository roomRepository
            )
        {
            m_roomRepository = roomRepository;
        }

        public EitherAsync<IError, IRoom> Execute(string roomId) =>
            m_roomRepository.FindById(roomId).ToAsync();
    }
}
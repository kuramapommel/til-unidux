using LanguageExt;
using Pommel.Server.Domain;
using Pommel.Server.Domain.InGame;
using static LanguageExt.Prelude;

namespace Pommel.Server.UseCase.InGame
{
    public interface IEnterRoomUseCase
    {
        EitherAsync<IError, IRoom> Execute(string roomId, string playerId, string playerName);
    }

    public sealed class EnterRoomUseCase : IEnterRoomUseCase
    {
        private readonly IRoomRepository m_roomRepository;

        private readonly IPlayerFactory m_playerFactory;

        public EnterRoomUseCase(
            IRoomRepository roomRepository,
            IPlayerFactory playerFactory
            )
        {
            m_roomRepository = roomRepository;
            m_playerFactory = playerFactory;
        }

        public EitherAsync<IError, IRoom> Execute(string roomId, string playerId, string playerName) =>
                from room in m_roomRepository.FindById(roomId).ToAsync()
                from player in Try(() => m_playerFactory.Create(playerId, playerName))
                    .ToEitherAsync()
                    .MapLeft(e => new DomainError(e) as IError)
                from entered in Try(() => room.Enter(player))
                    .ToEitherAsync()
                    .MapLeft(e => new DomainError(e) as IError)
                from savedRoom in m_roomRepository.Save(entered).ToAsync()
                select savedRoom;
    }
}
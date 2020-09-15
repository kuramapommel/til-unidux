using System;
using LanguageExt;
using Pommel.Server.Domain;
using Pommel.Server.Domain.InGame;
using static LanguageExt.Prelude;

namespace Pommel.Server.UseCase.InGame
{
    public interface ICreateRoomUseCase
    {
        EitherAsync<IError, IRoom> Execute();
    }

    public sealed class CreateRoomUseCase : ICreateRoomUseCase
    {
        private readonly IRoomFactory m_roomFactory;

        private readonly IRoomRepository m_roomRepository;

        public CreateRoomUseCase(
            IRoomFactory roomFactory,
            IRoomRepository roomRepository
            )
        {
            m_roomFactory = roomFactory;
            m_roomRepository = roomRepository;
        }

        public EitherAsync<IError, IRoom> Execute() =>
                from roomId in RightAsync<IError, string>(Guid.NewGuid().ToString("N")) // todo ID Generator 的なものをかませる
                from room in Try(() => m_roomFactory.Create(roomId))
                    .ToEitherAsync()
                    .MapLeft(e => new DomainError(e) as IError)
                from savedRoom in m_roomRepository.Save(room).ToAsync()
                select savedRoom;

    }
}
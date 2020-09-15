using System;
using System.Collections.Generic;
using Pommel.Server.Domain.InGame;

namespace Pommel.Server.Infrastructure.Store.InGame
{
    public interface IRoomStore : IDictionary<string, IRoom>
    {

    }

    public static class RoomStore
    {
        private static readonly Lazy<IRoomStore> m_instance = new Lazy<IRoomStore>(() => new Impl());

        public static IRoomStore Instance => m_instance.Value;

        private sealed class Impl : Dictionary<string, IRoom>, IRoomStore
        {
        }
    }
}
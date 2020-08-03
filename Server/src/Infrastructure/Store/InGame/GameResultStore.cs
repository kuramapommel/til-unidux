using System;
using System.Collections.Generic;
using Pommel.Server.Domain.InGame;

namespace Pommel.Server.Infrastructure.Store.InGame
{
    public interface IGameResultStore : IDictionary<string, IGameResult>
    {

    }

    public static class GameResultStore
    {
        private static readonly Lazy<IGameResultStore> m_instance = new Lazy<IGameResultStore>(() => new Impl());

        public static IGameResultStore Instance => m_instance.Value;

        private sealed class Impl : Dictionary<string, IGameResult>, IGameResultStore
        {
        }
    }
}
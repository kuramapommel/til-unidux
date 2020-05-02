using System;
using System.Collections.Generic;
using Pommel.Reversi.Domain.InGame;

namespace Pommel.Reversi.Infrastructure.Store.InGame
{
    public interface IGameStore : IDictionary<string, IGame>
    {

    }

    public static class GameStore
    {
        private static readonly Lazy<IGameStore> m_instance = new Lazy<IGameStore>(() => new Impl());

        public static IGameStore Instance => m_instance.Value;

        private sealed class Impl : Dictionary<string, IGame>, IGameStore
        {
        }
    }
}
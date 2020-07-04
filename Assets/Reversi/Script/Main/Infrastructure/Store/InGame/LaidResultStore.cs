using System;
using System.Collections.Generic;
using Pommel.Reversi.UseCase.InGame.Dto;

namespace Pommel.Reversi.Infrastructure.Store.InGame
{
    public interface ILaidResultStore : IDictionary<string, LaidDto>
    {

    }

    public static class LaidResultStore
    {
        private static readonly Lazy<ILaidResultStore> m_instance = new Lazy<ILaidResultStore>(() => new Impl());

        public static ILaidResultStore Instance => m_instance.Value;

        private sealed class Impl : Dictionary<string, LaidDto>, ILaidResultStore
        {
        }
    }
}
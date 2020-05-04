using System;
using System.Collections.Generic;
using Pommel.Reversi.UseCase.InGame.Dto;

namespace Pommel.Reversi.Infrastructure.Store.InGame
{
    public interface IGameResultStore : IDictionary<string, ResultDto>
    {

    }

    public static class GameResultStore
    {
        private static readonly Lazy<IGameResultStore> m_instance = new Lazy<IGameResultStore>(() => new Impl());

        public static IGameResultStore Instance => m_instance.Value;

        private sealed class Impl : Dictionary<string, ResultDto>, IGameResultStore
        {
        }
    }
}
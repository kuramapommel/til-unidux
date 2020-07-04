using System;

namespace Pommel.Reversi.Domain.InGame
{
    public interface IPlayer
    {
        string Id { get; }

        string Name { get; }
    }

    public interface IPlayerFactory
    {
        IPlayer Create(string id, string name);
    }

    public static class Player
    {
        public sealed class Impl : IPlayer
        {
            public string Id { get; }

            public string Name { get; }

            public Impl(string id, string name)
            {
                Id = id;
                Name = name;
            }
        }

        private static readonly Lazy<IPlayer> m_none = new Lazy<IPlayer>(() => new Impl(string.Empty, "---"));

        public static IPlayer None => m_none.Value;
    }
}
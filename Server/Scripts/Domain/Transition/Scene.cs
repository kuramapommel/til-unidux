using System;

namespace Pommel.Server.Domain.Transition
{
    public interface IScene
    {
        string Id { get; }

        bool IsBase { get; }
    }

    public static class Scene
    {
        private sealed class Impl : IScene
        {
            public string Id { get; }

            public bool IsBase { get; }

            public Impl(string id, bool isBase)
            {
                Id = id;
                IsBase = isBase;
            }
        }

        private static readonly Lazy<IScene> m_title = new Lazy<IScene>(() => new Impl("Title", false));

        private static readonly Lazy<IScene> m_inGame = new Lazy<IScene>(() => new Impl("InGame", false));

        public static IScene Title => m_title.Value;

        public static IScene InGame => m_inGame.Value;
    }
}
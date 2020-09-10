using System;

namespace Pommel.Reversi.Domain.Title
{
    public static class ValueObjects
    {
        [Serializable]
        public readonly struct Player
        {
            public string Id { get; }

            public string Name { get; }

            public Player(string id, string name)
            {
                Id = id;
                Name = name;
            }
        }
    }
}
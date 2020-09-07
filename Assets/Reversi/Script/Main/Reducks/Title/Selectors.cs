using System;

namespace Pommel.Reversi.Reducks.Title
{
    public static class Selectors
    {
        public static Func<StateRoot, bool> GetDisplayGameModal = state => state.Title.IsDisplayGameStartModal;
    }
}